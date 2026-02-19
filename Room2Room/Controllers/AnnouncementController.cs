using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models.Announcements;

namespace Room2Room.Controllers;

public class AnnouncementController : Controller
{
    // TODO: move out of controller, move db connection instantiation into factory
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Room2Room;User Id=sa;Password=aStrong!Passw0rd;TrustServerCertificate=True;";

    private readonly ApplicationDbContext _context;

    public AnnouncementController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Announcement/Active
    [HttpGet]
    public IActionResult Active()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        var now = DateTime.UtcNow;

        var announcement = context.Announcements
            .Where(a => a.IsActive && a.StartDate <= now && a.EndDate >= now)
            .OrderByDescending(a => a.StartDate)
            .FirstOrDefault();

        // Return max 1, or null
        return Json(announcement);
    }
}
