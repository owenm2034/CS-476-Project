using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Room2Room.Models;

namespace Room2Room.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult LogIn()
    {
        return View();
    }
}
