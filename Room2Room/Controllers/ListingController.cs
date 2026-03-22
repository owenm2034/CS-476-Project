using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Room2Room.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Room2Room.Services.Observers;
using Room2Room.Data;
using Room2Room.Models.Accounts;
using Room2Room.Models.Listings;
using Microsoft.EntityFrameworkCore;

namespace Room2Room.Controllers;

public class ListingController : Controller
{
    private readonly IListingRepository _listingRepository;
    private readonly IItemSubject _itemSubject;
    private readonly ApplicationDbContext _context;


    public ListingController(IListingRepository listingRepository, IItemSubject itemSubject, ApplicationDbContext context)
    {
        _listingRepository = listingRepository;
        _itemSubject = itemSubject;
        _context = context;
    }


    public async Task<IActionResult> Index(string sTerm = "", int? categoryId = null)
    {
        int? universityId = null;
        var universityClaim = User.Claims.FirstOrDefault(c => c.Type == "UniversityId");
        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");

        if (!string.IsNullOrEmpty(universityClaim?.Value))
        {
            universityId = int.Parse(universityClaim.Value);
        }

        IEnumerable<Item> items = await _listingRepository.GetItems(sTerm, categoryId, universityId);
        IEnumerable<Category> categories = await _listingRepository.GetCategories();

        var watchlistedItemIds = await _listingRepository.GetWatchlistedItemIdsAsync(userId);

        foreach (var item in items)
        {
            item.InWatchlist = watchlistedItemIds.Contains(item.Id);
        }

        // exclude the listings this account made from the page
        items = items.Where(x => x.AccountId != userId).ToList();

        ItemDisplayModel itemModel = new ItemDisplayModel
        {
            Items = items,
            Categories = categories,
            STerm = sTerm,
            CategoryId = categoryId
        };
        return View(itemModel);
    }

    [Authorize]
    public async Task<IActionResult> MyListings()
    {
        var accountIdClaim = User.FindFirstValue("AccountId");

        if (string.IsNullOrEmpty(accountIdClaim))
        {
            return RedirectToAction("LogIn", "Account");
        }

        int accountId = int.Parse(accountIdClaim);

        IEnumerable<Item> items = await _listingRepository.GetItemsByAccountId(accountId);

        return View(items);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        IEnumerable<Category> categories = await _listingRepository.GetCategories();
        ViewBag.Categories = categories;
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItem dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _listingRepository.GetCategories();
            return View(dto);
        }

        var accountIdClaim = User.FindFirstValue("AccountId");

        if (string.IsNullOrEmpty(accountIdClaim))
        {
            return RedirectToAction("LogIn", "Account");
        }

        int accountId = int.Parse(accountIdClaim);

        var itemToCreate = new Item
        {
            ItemName = dto.ItemName,
            ItemDescription = dto.ItemDescription,
            ItemPrice = dto.Price,
            CategoryId = dto.CategoryId,
            Status = "Active",
            AccountId = accountId,
            UniversityName = await _listingRepository.GetUniversityNameByAccountIdAsync(accountId)
        };
        await _listingRepository.AddItemAsync(itemToCreate);

        if (dto.Images != null && dto.Images.Count > 0)
        {
            foreach (var formFile in dto.Images)
            {
                if (formFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{formFile.FileName}";
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads",
                        fileName
                    );

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    await _listingRepository.AddItemImageAsync(
                        new ItemImage
                        {
                            ItemId = itemToCreate.Id,
                            ImagePath = "/uploads/" + fileName
                        }
                    );
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id, string? returnTo = null)
    {
        var item = await _listingRepository.GetItemById(id);

        if (item == null)
        {
            return NotFound();
        }

        var accountIdClaim = User.FindFirstValue("AccountId");

        if (string.IsNullOrEmpty(accountIdClaim))
        {
            return RedirectToAction("LogIn", "Account");
        }

        int currentUserId = int.Parse(accountIdClaim);

        if (item.AccountId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        ViewBag.Categories = await _listingRepository.GetCategories();

        ViewBag.StatusOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "Available", Text = "Available" },
            new SelectListItem { Value = "Sold", Text = "Sold" },
            new SelectListItem { Value = "On Hold", Text = "On Hold" }
        };

        var currentImage = await _listingRepository.GetFirstItemImageAsync(id);

        EditItem dto = new EditItem
        {
            Id = item.Id,
            ItemName = item.ItemName ?? "",
            ItemDescription = item.ItemDescription ?? "",
            Price = item.ItemPrice,
            CategoryId = item.CategoryId,
            Status = item.Status ?? "Available",
            ReturnTo = returnTo,
            CurrentImagePath = currentImage?.ImagePath
        };

        return View(dto);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditItem dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _listingRepository.GetCategories();
            ViewBag.StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Available", Text = "Available" },
                new SelectListItem { Value = "Sold",      Text = "Sold" },
                new SelectListItem { Value = "On Hold",   Text = "Reserved" }
            };
            var currentImage = await _listingRepository.GetFirstItemImageAsync(dto.Id);
            dto.CurrentImagePath = currentImage?.ImagePath;
            return View(dto);
        }

        var item = await _listingRepository.GetItemById(dto.Id);

        if (item == null)
        {
            return NotFound();
        }

        var accountIdClaim = User.FindFirstValue("AccountId");

        if (string.IsNullOrEmpty(accountIdClaim))
        {
            return RedirectToAction("LogIn", "Account");
        }

        int currentUserId = int.Parse(accountIdClaim);

        if (item.AccountId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // this is for observer pattern
        var oldItem = new Item
        {
            ItemPrice = item.ItemPrice,
            Status = item.Status
        };

        item.ItemName = dto.ItemName;
        item.ItemDescription = dto.ItemDescription;
        item.ItemPrice = dto.Price;
        item.CategoryId = dto.CategoryId;
        item.Status = dto.Status;

        await _listingRepository.UpdateItemAsync(item);
        
        // this is what triggers the whole observer pattern by sending the old state and the new state
        _itemSubject.UpdateItem(oldItem, item);

        if (dto.NewImage != null && dto.NewImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.NewImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await dto.NewImage.CopyToAsync(stream);
            }

            var existingImage = await _listingRepository.GetFirstItemImageAsync(dto.Id);

            if (existingImage != null)
            {
                var oldFilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    existingImage.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                existingImage.ImagePath = "/uploads/" + fileName;
                await _listingRepository.UpdateItemImageAsync(existingImage);
            }
            else
            {
                await _listingRepository.AddItemImageAsync(
                    new ItemImage
                    {
                        ItemId = item.Id,
                        ImagePath = "/uploads/" + fileName
                    }
                );
            }
        }

        if (!string.IsNullOrEmpty(dto.ReturnTo))
        {
            return LocalRedirect(dto.ReturnTo);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int listingId, string? returnTo = null)
    {
        var listing = await _listingRepository.GetItemById(listingId);

        if (listing == null)
        {
            return NotFound();
        }

        var accountIdClaim = User.FindFirstValue("AccountId");

        if (string.IsNullOrEmpty(accountIdClaim))
        {
            return RedirectToAction("LogIn", "Account");
        }

        int currentUserId = int.Parse(accountIdClaim);

        if (listing.AccountId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        await _listingRepository.Delete(listingId);

        if (!string.IsNullOrEmpty(returnTo))
        {
            return LocalRedirect(returnTo);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
{
    var item = await _context.Items
        .Include(i => i.ItemImage)
        .Include(i => i.Category)
        .Select(i => new Item
        {
            Id = i.Id,
            ItemName = i.ItemName,
            ItemDescription = i.ItemDescription,
            ItemPrice = i.ItemPrice,
            Status = i.Status,
            CategoryId = i.CategoryId,
            AccountId = i.AccountId,
            Category = i.Category,
            ItemImage = i.ItemImage
        })
        .FirstOrDefaultAsync(i => i.Id == id);
    
    if (item == null)
        return NotFound();
 
    var seller = await _context.Accounts.FindAsync(item.AccountId);
 
    var vm = new ListingDetail
{
        Listing = item,
        SellerName = seller?.Username ?? "Unknown",
        SellerEmail = seller?.Email ?? "",
        AllImages = item.ItemImage ?? new List<ItemImage>()
    };

return View(vm);
}

[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Report(int itemId, string reason)
{
    int accountId = int.Parse(User.FindFirstValue("AccountId") ?? "0");
    _context.ItemReports.Add(new ItemReport
    {
        ItemId = itemId,
        ReportedByAccountId = accountId,
        Reason = reason ?? "",
        CreatedAt = DateTime.Now
    });
    await _context.SaveChangesAsync();
    return Json(new { success = true });
}

}