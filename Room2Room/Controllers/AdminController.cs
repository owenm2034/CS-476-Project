using Microsoft.AspNetCore.Mvc;
using Room2Room.Data;
using Microsoft.AspNetCore.Authorization;

namespace Room2Room.Controllers;

[Authorize(Roles="Admin")]
public class AdminController : Controller
{
    // TODO: move out of controller, move db connection instantiation into factory
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Room2Room;User Id=sa;Password=aStrong!Passw0rd;TrustServerCertificate=True;";
  
    private readonly IListingRepository _listingRepository;

    public AdminController(IListingRepository homeRepository)
    {
        _listingRepository = homeRepository;
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
}
