using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Room2Room.Models.Listings;

[Table("SaveLaterDetail")]
public class SaveLaterDetail
{
    public int Id { get; set; }
    [Required]
    public int SaveLaterId { get; set; }
    [Required]
    public int ItemId { get; set; }
    public Item Item { get; set; }
    public SaveLater SaveLater { get; set; }

}