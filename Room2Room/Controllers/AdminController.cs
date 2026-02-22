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

    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
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
    public IActionResult CreateAnnouncement(string message, DateTime? startDate, DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(message))
            return BadRequest("Message is required.");

        var now = DateTime.Now;

        var finalStart = startDate ?? now;
        var finalEnd = endDate ?? DateTime.MaxValue;

        var nowMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        var startMinute = new DateTime(finalStart.Year, finalStart.Month, finalStart.Day, finalStart.Hour, finalStart.Minute, 0);

        if (startMinute == nowMinute)
            finalStart = now;
        else if (startMinute < nowMinute)
            return BadRequest("Start date must be now or later.");

        if (finalEnd <= finalStart)
            return BadRequest("End date must be after start date.");

        var announcement = new Announcement(message, "warning", finalStart, finalEnd);

        _context.Announcements.Add(announcement);
        _context.SaveChanges();

        return Ok();
    }

    // ===== Announcements Edit =====
    [HttpPost]
    public IActionResult EditAnnouncement(int id, string message, DateTime? startDate, DateTime? endDate)
    {
        var a = _context.Announcements.FirstOrDefault(x => x.Id == id);
        if (a == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(message))
            return BadRequest("Message is required.");

        var now = DateTime.Now;
        var nowMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        var finalStart = a.StartDate; // keep original seconds
        var finalEnd = a.EndDate;

        // Only validate if user actually changed the start time
        if (startDate.HasValue)
        {
            var incoming = startDate.Value;
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
        if (endDate.HasValue)
        {
            var incoming = endDate.Value;
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

        a.Message = message;
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