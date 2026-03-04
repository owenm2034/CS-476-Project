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
        return View(watchlistItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToWatchlist(int itemId) // Add an item to the watchlist
    {
        await _watchlistRepository.AddToWatchlistAsync(CurrentUserId, itemId); // Add the item to the user's watchlist
        return RedirectToAction("Index"); // Redirect back to the watchlist page
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromWatchlist(int itemId) // Remove an item from the watchlist
    {
        await _watchlistRepository.RemoveFromWatchlistAsync(CurrentUserId, itemId); // Remove the item from the user's watchlist
        return RedirectToAction("Index"); // Redirect back to the watchlist page
    }
}