using Room2Room.Models.Watchlist;

public interface IWatchlistRepository
{
    Task<IEnumerable<Watchlist>> GetByUserIdAsync(int userId);
    Task<bool> IsInWatchlistAsync(int userId, int itemId);  //check if item is already in watchlist
    Task AddToWatchlist(int userId, int itemId);
    Task RemoveFromWatchlist(int userId, int itemId);
}