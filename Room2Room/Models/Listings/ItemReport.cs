using Room2Room.Models.Accounts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Room2Room.Models.Listings;

[Table("ItemReport")]
public class ItemReport
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int ReportedByAccountId { get; set; }
    public string Reason { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Resolution { get; set; } = null;
    public DateTime? ResolvedAt { get; set; } = null;

    [NotMapped]
    public Item? Item { get; set; }
    [NotMapped]
    public Account? Reporter { get; set; }
}