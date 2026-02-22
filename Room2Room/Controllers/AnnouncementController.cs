using Microsoft.AspNetCore.Mvc;
using Room2Room.Data;

namespace Room2Room.Controllers;

public class AnnouncementController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnnouncementController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Announcement/Active
    [HttpGet]
    public IActionResult Active()
    {
        var now = DateTime.Now;

        var announcement = _context.Announcements
            .Where(a => a.IsActive && a.StartDate <= now && a.EndDate >= now)
            .OrderByDescending(a => a.StartDate)
            .FirstOrDefault();

        return Json(announcement);
    }
}