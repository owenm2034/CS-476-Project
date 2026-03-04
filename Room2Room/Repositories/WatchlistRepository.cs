using Microsoft.EntityFrameworkCore;
using Room2Room.Models.Watchlist;
public class WatchlistRepository : IWatchlistRepository
{
    private readonly ApplicationDbContext _context;

    public WatchlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Watchlist>> GetByUserIdAsync(int userId)
    {
        return await _context.Watchlists
        .Where(w => w.UserId == userId)
        .Include(w => w.Item)
        .OrderByDescending(w => w.DateAdded)
        .ToListAsync();
    }

    public async Task<bool> IsInWatchlistAsync(int userId, int itemId)
    {
        return await _context.Watchlists.AnyAsync(w => w.UserId == userId && w.ItemId == itemId);
    }

    public async Task AddToWatchlist(int userId, int itemId)
    {
        if (await IsInWatchlistAsync(userId, itemId))
        {
            return; // Item is already in the watchlist, do not add again
        }

        var watchlistEntry = new Watchlist { UserId = userId, ItemId = itemId, DateAdded = DateTime.UtcNow };
        _context.Watchlists.Add(watchlistEntry);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromWatchlist(int userId, int itemId)
    {
        var watchlistEntry = await _context.Watchlists.FirstOrDefaultAsync(w => w.UserId == userId && w.ItemId == itemId);
        if (watchlistEntry != null)
        {
            _context.Watchlists.Remove(watchlistEntry);
            await _context.SaveChangesAsync();
        }
    }
}