using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models;
using Room2Room.Models.Accounts;
using Room2Room.Models.Announcements;

namespace Room2Room.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IListingRepository _listingRepository;
    private readonly ApplicationDbContext _context;

    public AdminController(IListingRepository homeRepository, ApplicationDbContext context)
    {
        _listingRepository = homeRepository;
        _context = context;
    }

    public async Task<IActionResult> Listings(
        string sTerm = "",
        int? categoryId = null,
        int? universityId = null
    )
    {
        var items =
            (await _listingRepository.GetItems(sTerm, categoryId, universityId))
            ?? new List<Item>();

        var categories =
            (await _listingRepository.GetCategories())
            ?? new List<Category>();

        var universities = (
            from item in _context.Items
            join account in _context.Accounts on item.AccountId equals account.Id
            join university in _context.Universities on account.UniversityId equals university.Id
            select university
        ).Distinct();

        var itemModel = new ItemDisplayModel
        {
            Items = items
                .OrderBy(x => x.UniversityName)
                .ThenBy(x => x.Category)
                .ThenBy(x => x.ItemPrice),
            Categories = categories,
            Universities = universities,
            STerm = sTerm ?? string.Empty,
            CategoryId = categoryId,
            UniversityId = universityId
        };

        return PartialView("_AdminListings", itemModel);
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Users(string sTerm = "")
    {
        var normalizedSearch = (sTerm ?? string.Empty).Trim();

        var query =
            from account in _context.Accounts
            join university in _context.Universities on account.UniversityId equals university.Id into universityJoin
            from university in universityJoin.DefaultIfEmpty()
            join restriction in _context.AccountRestrictions on account.Id equals restriction.AccountId into restrictionJoin
            from restriction in restrictionJoin.DefaultIfEmpty()
            select new AdminUserListItem
            {
                Id = account.Id,
                Email = account.Email,
                Username = account.Username,
                IsAdmin = account.IsAdmin,
                UniversityName = university != null ? university.Name : "Unknown",
                AccountStatus = restriction != null ? restriction.Status : "Active"
            };

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(x =>
                x.Email.Contains(normalizedSearch) ||
                x.Username.Contains(normalizedSearch) ||
                x.UniversityName.Contains(normalizedSearch)
            );
        }

        var users = await query
            .OrderByDescending(x => x.IsAdmin)
            .ThenBy(x => x.Username)
            .ToListAsync();

        var model = new AdminUsersViewModel
        {
            Users = users,
            STerm = normalizedSearch
        };

        return PartialView("_Users", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserStatus(int id, string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
        {
            return BadRequest("Invalid status.");
        }

        var validStatuses = new[] { "Active", "Suspended", "Deactivated" };
        if (!validStatuses.Contains(newStatus))
        {
            return BadRequest("Invalid status.");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
        if (account == null)
        {
            return NotFound();
        }

        var currentAdminIdClaim = User.FindFirst("AccountId")?.Value;
        if (int.TryParse(currentAdminIdClaim, out var currentAdminId) &&
            account.Id == currentAdminId &&
            newStatus != "Active")
        {
            return BadRequest("You cannot suspend or deactivate your own account.");
        }

        var restriction = await _context.AccountRestrictions
            .FirstOrDefaultAsync(x => x.AccountId == id);

        if (newStatus == "Active")
        {
            if (restriction != null)
            {
                _context.AccountRestrictions.Remove(restriction);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        if (restriction == null)
        {
            restriction = new AccountRestriction
            {
                AccountId = id,
                Status = newStatus,
                UpdatedAt = DateTime.UtcNow
            };

            _context.AccountRestrictions.Add(restriction);
        }
        else
        {
            restriction.Status = newStatus;
            restriction.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
        if (account == null)
        {
            return NotFound();
        }

        var model = new AdminEditUserModel
        {
            Id = account.Id,
            Email = account.Email,
            Username = account.Username,
            IsAdmin = account.IsAdmin
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(AdminEditUserModel model)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (account == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains("@"))
        {
            ModelState.AddModelError(nameof(model.Email), "A valid email is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Username is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var newEmail = model.Email.Trim();
        var newUsername = model.Username.Trim();

        if (!newEmail.Equals(account.Email, StringComparison.OrdinalIgnoreCase))
        {
            var newEmailLower = newEmail.ToLower();

            var emailExists = await _context.Accounts.AnyAsync(x =>
                x.Id != account.Id && x.Email.ToLower() == newEmailLower
            );

            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
                return View(model);
            }

            var domainStart = newEmail.IndexOf("@", StringComparison.Ordinal);
            var domain = domainStart >= 0 ? newEmail[(domainStart + 1)..] : string.Empty;

            var university = await _context.Universities
                .FirstOrDefaultAsync(x => x.Domain == domain);

            if (university == null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email must use a valid university domain.");
                return View(model);
            }

            account.Email = newEmail;
            account.UniversityId = university.Id;
        }

        if (!newUsername.Equals(account.Username, StringComparison.OrdinalIgnoreCase))
        {
            var newUsernameLower = newUsername.ToLower();

            var usernameExists = await _context.Accounts.AnyAsync(x =>
                x.Id != account.Id && x.Username.ToLower() == newUsernameLower
            );

            if (usernameExists)
            {
                ModelState.AddModelError(nameof(model.Username), "An account with this username already exists.");
                return View(model);
            }

            account.Username = newUsername;
        }

        account.IsAdmin = model.IsAdmin;
        await _context.SaveChangesAsync();

        TempData["AdminUserEditSuccess"] = "Account updated successfully.";
        return RedirectToAction(nameof(EditUser), new { id = model.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Statistics()
    {
        var totalUsers = await _context.Accounts.CountAsync();
        var totalListings = await _context.Items.CountAsync();

        var listingsByUniversity = await (
            from item in _context.Items
            join account in _context.Accounts on item.AccountId equals account.Id
            join university in _context.Universities on account.UniversityId equals university.Id
            group item by university.Name into g
            orderby g.Count() descending, g.Key
            select new UniversityListingStat
            {
                UniversityName = g.Key,
                ListingCount = g.Count()
            }
        ).ToListAsync();

        var listingsByCategory = await (
            from item in _context.Items
            join category in _context.Categories on item.CategoryId equals category.Id
            group item by category.CategoryName into g
            orderby g.Count() descending, g.Key
            select new CategoryListingStat
            {
                CategoryName = g.Key,
                ListingCount = g.Count()
            }
        ).ToListAsync();

        var topUsersByListings = await (
            from item in _context.Items
            join account in _context.Accounts on item.AccountId equals account.Id
            group item by new
            {
                account.Id,
                account.Email
            } into g
            orderby g.Count() descending
            select new TopUserListingStat
            {
                UserDisplayName = g.Key.Email,
                ListingCount = g.Count()
            }
        ).Take(3).ToListAsync();

        var averageListingsPerUser = totalUsers == 0
            ? 0
            : Math.Round((double)totalListings / totalUsers, 1);

        var model = new AdminStatisticsViewModel
        {
            TotalUsers = totalUsers,
            TotalListings = totalListings,
            UniversitiesWithListings = listingsByUniversity.Count,
            AverageListingsPerUser = averageListingsPerUser,
            ListingsByUniversity = listingsByUniversity,
            ListingsByCategory = listingsByCategory,
            TopUsersByListings = topUsersByListings
        };

        return PartialView("_Statistics", model);
    }

    [HttpGet]
    public async Task<IActionResult> ListAnnouncements()
    {
        var announcements = await _context.Announcements
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();

        return PartialView("_AnnouncementsList", announcements);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAnnouncement(AnnouncementFormModel model)
    {
        if (model == null)
            return BadRequest("Invalid request.");

        if (string.IsNullOrWhiteSpace(model.Message))
            return BadRequest("Message is required.");

        var now = DateTime.Now;

        var finalStart = model.StartDate ?? now;
        var finalEnd = model.EndDate ?? DateTime.MaxValue;

        var nowMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        var startMinute = new DateTime(
            finalStart.Year,
            finalStart.Month,
            finalStart.Day,
            finalStart.Hour,
            finalStart.Minute,
            0
        );

        if (startMinute == nowMinute)
            finalStart = now;
        else if (startMinute < nowMinute)
            return BadRequest("Start date must be now or later.");

        if (finalEnd <= finalStart)
            return BadRequest("End date must be after start date.");

        var announcement = new Announcement(model.Message.Trim(), "warning", finalStart, finalEnd);

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> EditAnnouncement(AnnouncementFormModel model)
    {
        if (model == null)
            return BadRequest("Invalid request.");

        var a = await _context.Announcements.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (a == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(model.Message))
            return BadRequest("Message is required.");

        var now = DateTime.Now;
        var nowMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        var finalStart = a.StartDate;
        var finalEnd = a.EndDate;

        if (model.StartDate.HasValue)
        {
            var incoming = model.StartDate.Value;
            var incomingMinute = new DateTime(
                incoming.Year,
                incoming.Month,
                incoming.Day,
                incoming.Hour,
                incoming.Minute,
                0
            );

            var existing = a.StartDate;
            var existingMinute = new DateTime(
                existing.Year,
                existing.Month,
                existing.Day,
                existing.Hour,
                existing.Minute,
                0
            );

            if (incomingMinute != existingMinute)
            {
                if (incomingMinute == nowMinute)
                    finalStart = now;
                else if (incomingMinute < nowMinute)
                    return BadRequest("Start date must be now or later.");
                else
                    finalStart = incoming;
            }
        }

        if (model.EndDate.HasValue)
        {
            var incoming = model.EndDate.Value;
            var incomingMinute = new DateTime(
                incoming.Year,
                incoming.Month,
                incoming.Day,
                incoming.Hour,
                incoming.Minute,
                0
            );

            var existing = a.EndDate;
            var existingMinute = new DateTime(
                existing.Year,
                existing.Month,
                existing.Day,
                existing.Hour,
                existing.Minute,
                0
            );

            if (incomingMinute != existingMinute)
            {
                finalEnd = incoming;
            }
        }

        if (finalEnd <= finalStart)
            return BadRequest("End date must be after start date.");

        a.Message = model.Message.Trim();
        a.StartDate = finalStart;
        a.EndDate = finalEnd;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ToggleAnnouncement(int id)
    {
        var a = await _context.Announcements.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null)
            return NotFound();

        a.IsActive = !a.IsActive;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAnnouncement(int id)
    {
        var a = await _context.Announcements.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null)
            return NotFound();

        _context.Announcements.Remove(a);
        await _context.SaveChangesAsync();

        return Ok();
    }

    public async Task<IActionResult> GetCategories()
    {
        var categories = (await _listingRepository.GetCategories()) ?? new List<Category>();
        categories = categories.OrderBy(x => x.CategoryName).ToList();
        return PartialView("_AdminCategories", categories);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertCategory(Category? cat)
    {
        if (cat == null)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(cat.CategoryName))
        {
            return BadRequest();
        }

        cat.CategoryName = cat.CategoryName.Trim();

        if (cat.Id == 0)
        {
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            return Ok();
        }

        var oldCat = await _context.Categories.FirstOrDefaultAsync(x => x.Id == cat.Id);
        if (oldCat == null)
        {
            return NotFound();
        }

        oldCat.CategoryName = cat.CategoryName;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int catId)
    {
        var oldCat = await _context.Categories.FirstOrDefaultAsync(x => x.Id == catId);
        if (oldCat == null)
        {
            return BadRequest();
        }

        var itemIds = await _context.Items
            .Where(x => x.CategoryId == oldCat.Id)
            .Select(x => x.Id)
            .ToListAsync();

        var watchlists = await _context.Watchlists
            .Where(x => itemIds.Contains(x.ItemId))
            .ToListAsync();

        _context.RemoveRange(watchlists);
        await _context.SaveChangesAsync();

        _context.Categories.Remove(oldCat);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
public async Task<IActionResult> ListReports()
{
    var reports = await _context.ItemReports
        .OrderByDescending(r => r.CreatedAt)
        .Select(r => new ItemReport
        {
            Id = r.Id,
            ItemId = r.ItemId,
            ReportedByAccountId = r.ReportedByAccountId,
            Reason = r.Reason,
            CreatedAt = r.CreatedAt,
            Resolution = r.Resolution,
            ResolvedAt = r.ResolvedAt,  
            Item = _context.Items
                .Where(i => i.Id == r.ItemId)
                .Select(i => new Item { Id = i.Id, ItemName = i.ItemName, AccountId = i.AccountId, Status = i.Status, CategoryId = i.CategoryId, ItemPrice = i.ItemPrice })
                .FirstOrDefault(),
            Reporter = _context.Accounts
                .Where(a => a.Id == r.ReportedByAccountId)
                .Select(a => new Account(a.Email, a.PasswordHash, a.PasswordSalt, a.Username, a.ProfilePictureUrl, a.UniversityId) { Id = a.Id })
                .FirstOrDefault()
        })
        .ToListAsync();

    return PartialView("_ReportedListings", reports);
}

[HttpPost]
public async Task<IActionResult> ResolveReport(int id, string resolution)
{
    var report = await _context.ItemReports.FirstOrDefaultAsync(r => r.Id == id);
    if (report == null) return NotFound();

    report.Resolution = resolution;
    report.ResolvedAt = DateTime.Now;
    await _context.SaveChangesAsync();
    return Ok();
}

[HttpGet]
public async Task<IActionResult> ListUserReports()
{
    var reports = await _context.UserReports
        .OrderByDescending(r => r.CreatedAt)
        .Select(r => new UserReport
        {
            Id = r.Id,
            ReportedAccountId = r.ReportedAccountId,
            ReportedByAccountId = r.ReportedByAccountId,
            Reason = r.Reason,
            CreatedAt = r.CreatedAt,
            Resolution = r.Resolution,
            ResolvedAt = r.ResolvedAt,
            ReportedAccount = _context.Accounts
                .Where(a => a.Id == r.ReportedAccountId)
                .Select(a => new Account(a.Email, a.PasswordHash, a.PasswordSalt, a.Username, a.ProfilePictureUrl, a.UniversityId) { Id = a.Id })
                .FirstOrDefault(),
            Reporter = _context.Accounts
                .Where(a => a.Id == r.ReportedByAccountId)
                .Select(a => new Account(a.Email, a.PasswordHash, a.PasswordSalt, a.Username, a.ProfilePictureUrl, a.UniversityId) { Id = a.Id })
                .FirstOrDefault()
        })
        .ToListAsync();

    return PartialView("_ReportedUsers", reports);
}

[HttpPost]
public async Task<IActionResult> ResolveUserReport(int id, string resolution)
{
    var report = await _context.UserReports.FirstOrDefaultAsync(r => r.Id == id);
    if (report == null) return NotFound();

    report.Resolution = resolution;
    report.ResolvedAt = DateTime.Now;
    await _context.SaveChangesAsync();
    return Ok();
}
}