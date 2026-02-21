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

    public async Task<IActionResult> Create()
    {
        IEnumerable<Category> categories = await _homeRepository.GetCategories();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItem dto)
    {
        if (!ModelState.IsValid)
    {
        ViewBag.Categories = await _homeRepository.GetCategories();
        return View(dto);
    }

    var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
    if (string.IsNullOrEmpty(accountIdClaim))
    {
        return RedirectToAction("Login", "Account"); // user not logged in
    }
        int accountId = int.Parse(accountIdClaim);

        var itemToCreate = new Item
    {
        ItemName = dto.ItemName,
        ItemDescription = dto.ItemDescription,
        ItemPrice = dto.Price,
        CategoryId = dto.CategoryId,
        Status = "Active",
        AccountId = accountId
    };
            await _homeRepository.AddItemAsync(itemToCreate);

    if (dto.Images != null && dto.Images.Count > 0)
    {
        foreach (var formFile in dto.Images)
        {
            if (formFile.Length > 0)
            {
                // Generate a unique file name
                var fileName = $"{Guid.NewGuid()}_{formFile.FileName}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                // Save image path to database
                await _homeRepository.AddItemImageAsync(new ItemImage
                {
                    ItemId = itemToCreate.Id,
                    ImagePath = "/uploads/" + fileName
                });
            }
        }
    }

    return RedirectToAction(nameof(Index));
}
}
