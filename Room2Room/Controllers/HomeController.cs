using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Room2Room.Models;

namespace Room2Room.Controllers;

public class HomeController : Controller
{
    public HomeController() { }

    public IActionResult Index()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return Redirect("Listing/Index");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
