using Microsoft.EntityFrameworkCore;
using Room2Room.Models.Watchlist;
public class WatchlistRepository : IWatchlistRepository
{
    private readonly ApplicationDbContext _context;

    public WatchlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WatchListDisplayModel>> GetByUserIdAsync(int userId)
    {
        return await _context.Watchlists
        .Where(w => w.UserId == userId)
        .Select(w => new WatchListDisplayModel
        {
            WatchlistId = w.Id,
            ItemId = w.ItemId,
            ItemName = w.Item.ItemName,
            ItemDescription = w.Item.ItemDescription,
            ItemPrice = w.Item.ItemPrice,
            Status = w.Item.Status,
            ImagePath = w.Item.ItemImage
                            .OrderBy(img => img.Id) // Ensure consistent ordering
                            .Select(img => img.ImagePath)
                            .FirstOrDefault() // Get the first image path or null if no images        
        })
        .ToListAsync();
    }

    public async Task<bool> IsInWatchlistAsync(int userId, int itemId)
    {
        return await _context.Watchlists
        .AnyAsync(w => w.UserId == userId && w.ItemId == itemId);
    }

    public async Task AddToWatchlistAsync(int userId, int itemId)
    {
        bool exists = await IsInWatchlistAsync(userId, itemId);
        if (!exists)
        {
            var watchlistEntry = new Watchlist
            {
                UserId = userId,
                ItemId = itemId,
                DateAdded = DateTime.UtcNow
            };

            _context.Watchlists.Add(watchlistEntry);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromWatchlistAsync(int userId, int itemId)
    {
        var watchlistEntry = await _context.Watchlists
        .FirstOrDefaultAsync(w => w.UserId == userId && w.ItemId == itemId);

        if (watchlistEntry != null)
        {
            _context.Watchlists.Remove(watchlistEntry);
            await _context.SaveChangesAsync();
        }
    }
}