
public class AddWatchListDto    // DTO for adding an item to the watchlist
{
    public int ItemId { get; set; }
    public int AccountId { get; set; }
}

public class WatchListDto    // DTO for displaying the watchlist items
{
    public int WatchlistId { get; set; }
    public int ItemId { get; set; }
}