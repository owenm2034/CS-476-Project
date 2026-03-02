using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models;
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


    public async Task<IActionResult> Listings(string sTerm = "", int? categoryId = null, int? universityId = null)
    {
        var items = (await _listingRepository.GetItems(sTerm, categoryId, universityId)) ?? new List<Item>();
        var categories = (await _listingRepository.GetCategories()) ?? new List<Category>();
        var universities = 
            (from item in _context.Items
            join account in _context.Accounts on item.AccountId equals account.Id
            join university in _context.Universities on account.UniversityId equals university.Id
            select university).Distinct();

        var itemModel = new ItemDisplayModel
        {
            Items = items.OrderBy(x => x.UniversityName).ThenBy(x => x.Category).ThenBy(x => x.ItemPrice),
            Categories = categories,
            Universities = universities,
            STerm = sTerm?? string.Empty,
            CategoryId = categoryId,
            UniversityId = universityId
        };
        return PartialView("_AdminListings", itemModel);
    }
    public IActionResult Index()
    {
        return View();
    }

    // ===== Announcements List =====
    [HttpGet]
    public IActionResult ListAnnouncements()
    {
        var announcements = _context.Announcements
            .OrderByDescending(a => a.StartDate)
            .ToList();

        return PartialView("_AnnouncementsList", announcements);
    }

    // ===== Announcements Create =====
    [HttpPost]
    public IActionResult CreateAnnouncement(AnnouncementFormModel model)
    {
        if (model == null)
            return BadRequest("Invalid request.");

        if (string.IsNullOrWhiteSpace(model.Message))
            return BadRequest("Message is required.");

        var now = DateTime.Now;

        // Rule:
        // - If date/time not provided => show immediately
        // - If end not provided => never expires
        var finalStart = model.StartDate ?? now;
        var finalEnd = model.EndDate ?? DateTime.MaxValue;

        var nowMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        var startMinute = new DateTime(finalStart.Year, finalStart.Month, finalStart.Day, finalStart.Hour, finalStart.Minute, 0);

        if (startMinute == nowMinute)
            finalStart = now;
        else if (startMinute < nowMinute)
            return BadRequest("Start date must be now or later.");

        if (finalEnd <= finalStart)
            return BadRequest("End date must be after start date.");

        var announcement = new Announcement(model.Message.Trim(), "warning", finalStart, finalEnd);

        _context.Announcements.Add(announcement);
        _context.SaveChanges();

        return Ok();
    }

    // ===== Announcements Edit =====
    [HttpPost]
    public IActionResult EditAnnouncement(AnnouncementFormModel model)
    {
        if (model == null)
            return BadRequest("Invalid request.");

        var a = _context.Announcements.FirstOrDefault(x => x.Id == model.Id);
        if (a == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(model.Message))
            return BadRequest("Message is required.");

        var now = DateTime.Now;
        var nowMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        var finalStart = a.StartDate; // keep original seconds
        var finalEnd = a.EndDate;

        // Only validate if user actually changed the start time
        if (model.StartDate.HasValue)
        {
            var incoming = model.StartDate.Value;
            var incomingMinute = new DateTime(incoming.Year, incoming.Month, incoming.Day, incoming.Hour, incoming.Minute, 0);

            var existing = a.StartDate;
            var existingMinute = new DateTime(existing.Year, existing.Month, existing.Day, existing.Hour, existing.Minute, 0);

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

        // Only update end if user actually changed it
        if (model.EndDate.HasValue)
        {
            var incoming = model.EndDate.Value;
            var incomingMinute = new DateTime(incoming.Year, incoming.Month, incoming.Day, incoming.Hour, incoming.Minute, 0);

            var existing = a.EndDate;
            var existingMinute = new DateTime(existing.Year, existing.Month, existing.Day, existing.Hour, existing.Minute, 0);

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

        _context.SaveChanges();

        return Ok();
    }

    // ===== Announcements Deactivate/Activate =====
    [HttpPost]
    public IActionResult ToggleAnnouncement(int id)
    {
        var a = _context.Announcements.FirstOrDefault(x => x.Id == id);
        if (a == null)
            return NotFound();

        a.IsActive = !a.IsActive;
        _context.SaveChanges();

        return Ok();
    }

    // ===== Announcements Delete =====
    [HttpPost]
    public IActionResult DeleteAnnouncement(int id)
    {
        var a = _context.Announcements.FirstOrDefault(x => x.Id == id);
        if (a == null)
            return NotFound();

        _context.Announcements.Remove(a);
        _context.SaveChanges();

        return Ok();
    }
}