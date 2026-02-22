using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Room2Room.Controllers;


public class ListingController : Controller
{
    private readonly IListingRepository _listingRepository;

    public ListingController(IListingRepository homeRepository)
    {
        _listingRepository = homeRepository;
    }

    public async Task<IActionResult> Index(string sTerm = "", int categoryId = 0)
    {
        IEnumerable<Item> items = await _listingRepository.GetItems(sTerm, categoryId);
        IEnumerable<Category> categories = await _listingRepository.GetCategories();
        ItemDisplayModel itemModel = new ItemDisplayModel
        {
            Items = items,
            Categories = categories,
            STerm = sTerm,
            CategoryId = categoryId
        };
        return View(itemModel);
    }

    public async Task<IActionResult> Create()
    {
        IEnumerable<Category> categories = await _listingRepository.GetCategories();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItem dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _listingRepository.GetCategories();
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
        await _listingRepository.AddItemAsync(itemToCreate);

        if (dto.Images != null && dto.Images.Count > 0)
        {
            foreach (var formFile in dto.Images)
            {
                if (formFile.Length > 0)
                {
                    // Generate a unique file name
                    var fileName = $"{Guid.NewGuid()}_{formFile.FileName}";
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads",
                        fileName
                    );

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    // Save image path to database
                    await _listingRepository.AddItemImageAsync(
                        new ItemImage
                        {
                            ItemId = itemToCreate.Id,
                            ImagePath = "/uploads/" + fileName
                        }
                    );
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int listingId)
    {
        var listing = await _listingRepository.GetItemById(listingId);

        if (listing == null)
        {
            return NotFound();
        }

        if (
            ((ClaimsIdentity)User.Identity).FindFirst("AccountId").Value
                == listing.AccountId.ToString()
            || User.IsInRole("Admin")
        )
        {
            await _listingRepository.Delete(listingId);
        }

        return Redirect("/Listing/Index");
    }
}
