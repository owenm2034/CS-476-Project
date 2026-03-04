using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;

namespace Room2Room.Controllers;

public class AnnouncementController : Controller
{
    // TODO: move out of controller, move db connection instantiation into factory
    private string ConnectionString;

    private readonly ApplicationDbContext _context;

    public AnnouncementController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
    ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // GET: /Announcement/Active
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Active()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        var now = DateTime.Now;

        var announcement = context.Announcements
            .Where(a => a.IsActive && a.StartDate <= now && a.EndDate >= now)
            .OrderByDescending(a => a.StartDate)
            .FirstOrDefault();

        return Json(announcement);
    }
}