using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Room2Room.Models;

namespace Room2Room.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        return NotFound();
    }

    public IActionResult LogIn()
    {
        return View("LogIn");
    }

    public IActionResult Register()
    {
        return View("Register");
    }
}
