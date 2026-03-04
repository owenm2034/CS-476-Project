using Room2Room.Models.Watchlist;

public interface IWatchlistRepository
{
    Task<IEnumerable<WatchListDisplayModel>> GetByUserIdAsync(int userId);
    Task<bool> IsInWatchlistAsync(int userId, int itemId);  //check if item is already in watchlist
    Task AddToWatchlistAsync(int userId, int itemId);
    Task RemoveFromWatchlistAsync(int userId, int itemId);
}