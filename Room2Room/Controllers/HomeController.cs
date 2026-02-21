using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Room2Room.Models;
using Room2Room.Models.DTOs;

namespace Room2Room.Controllers;

public class HomeController : Controller
{
    
    private readonly IHomeRepository _homeRepository; // This line declares a private readonly field of type IHomeRepository, which is used to access the methods defined in the IHomeRepository interface. This allows the HomeController to interact with the data layer through the repository pattern, promoting separation of concerns and making it easier to manage data access logic.   
    
    public HomeController(IHomeRepository homeRepository)
    {
        _homeRepository = homeRepository; // This line assigns the injected IHomeRepository instance to the private field _homeRepository, enabling the controller to use the repository's methods to interact with the data layer. This is a common practice in ASP.NET Core applications to promote dependency injection and separation of concerns.
    }
    
    
    public async Task<IActionResult> Index(string sTerm="", int categoryId=0)
    {
        IEnumerable<Item> items = await _homeRepository.GetItems(sTerm, categoryId);
        IEnumerable<Category> categories = await _homeRepository.GetCategories();
        ItemDisplayModel itemModel = new ItemDisplayModel
        {
            Items = items,
            Categories = categories,
            STerm = sTerm,
            CategoryId = categoryId


        };
        return View(itemModel);
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
