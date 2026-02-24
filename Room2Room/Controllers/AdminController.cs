using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Room2Room.Data;
using Room2Room.Models.Announcements;

namespace Room2Room.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // TODO: move out of controller, move db connection instantiation into factory
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Room2Room;User Id=sa;Password=aStrong!Passw0rd;TrustServerCertificate=True;";
  

    private readonly IListingRepository _listingRepository;
    private readonly ApplicationDbContext _context;

    public AdminController(IListingRepository homeRepository, ApplicationDbContext context)
    {
        _listingRepository = homeRepository;
        _context = context;
    }


    public async Task<IActionResult> Listings(string sTerm = "", int categoryId = 0)
    {
        var items = (await _listingRepository.GetAllItemsForAdmin()) ?? new List<Item>();
        var categories = (await _listingRepository.GetCategories()) ?? new List<Category>();

        var itemModel = new ItemDisplayModel
        {
            Items = items,
            Categories = categories,
            STerm = sTerm?? string.Empty,
            CategoryId = categoryId
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

    public class AnnouncementFormModel
    {
        public int Id { get; set; }               // used for Edit
        public string? Message { get; set; }
        public DateTime? StartDate { get; set; }  // optional
        public DateTime? EndDate { get; set; }    // optional
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