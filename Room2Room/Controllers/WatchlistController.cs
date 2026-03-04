using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize] // Ensure that only authenticated users can access the watchlist
public class WatchlistController : Controller
{
    private readonly IWatchlistRepository _watchlistRepository;

    public WatchlistController(IWatchlistRepository watchlistRepository)
    {
        _watchlistRepository = watchlistRepository;
    }

    private int CurrentUserId
    {
        get
        {
            var claimValue = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
            if (claimValue == null) throw new Exception("User is not logged in or claim missing");
            return int.Parse(claimValue);
        }
    }
    
    public async Task<IActionResult> Index() // Display the user's watchlist
    {
        var watchlistItems = await _watchlistRepository.GetByUserIdAsync(CurrentUserId); // Fetch the watchlist items for the user
        var displayModel = watchlistItems.Select(w => new WatchListDisplayModel
        {
            ItemId = w.ItemId,
        }).ToList();
        return View(displayModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToWatchlist(int itemId) // Add an item to the watchlist
    {
        await _watchlistRepository.AddToWatchlist(CurrentUserId, itemId); // Add the item to the user's watchlist
        return Json(new { success = true }); // Return a JSON response indicating success
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromWatchlist(int itemId) // Remove an item from the watchlist
    {
        await _watchlistRepository.RemoveFromWatchlist(CurrentUserId, itemId); // Remove the item from the user's watchlist
        return RedirectToAction("Index"); // Redirect back to the watchlist page
    }
}