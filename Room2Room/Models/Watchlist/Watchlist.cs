namespace Room2Room.Models.Watchlist;
using Room2Room.Models.Accounts;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Watchlist")]
public class Watchlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.Now;

    public Account User { get; set; } = null!;
    public Item Item { get; set; } = null!;

}