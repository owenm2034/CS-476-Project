using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Room2Room.Repositories;

namespace Room2Room.Controllers;

public class ListingController : Controller
{
    private readonly IListingRepository _listingRepository;

    public ListingController(IListingRepository homeRepository)
    {
        _listingRepository = homeRepository;
    }

    public async Task<IActionResult> Index(string sTerm = "", int? categoryId = null)
    {
        int? universityId = null;
        var universityClaim = User.Claims.FirstOrDefault(c => c.Type == "UniversityId");
        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");

        if (!string.IsNullOrEmpty(universityClaim?.Value))
        {
            universityId = int.Parse(universityClaim.Value);
        }

        IEnumerable<Item> items = await _listingRepository.GetItems(sTerm, categoryId, universityId);
        IEnumerable<Category> categories = await _listingRepository.GetCategories();

        var watchlistedItemIds = await _listingRepository.GetWatchlistedItemIdsAsync(userId);

        foreach (var item in items)
        {
            item.InWatchlist = watchlistedItemIds.Contains(item.Id);
        }
        
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
            AccountId = accountId,
            UniversityName = await _listingRepository.GetUniversityNameByAccountIdAsync(accountId)
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

    public async Task<IActionResult> Delete(int listingId, string? returnTo = null)
    {
        var listing = await _listingRepository.GetItemById(listingId);

        if (listing == null)
        {
            return NotFound();
        }

        if (
            (((ClaimsIdentity)User?.Identity)?.FindFirst("AccountId").Value ?? "not an id")
                == listing.AccountId.ToString()
            || User.IsInRole("Admin")
        )
        {
            await _listingRepository.Delete(listingId);
        }

        if (User.IsInRole("Admin") && returnTo != null)
        {
            return Redirect(returnTo);
        }

        return Redirect("/Listing/Index");
    }
}
