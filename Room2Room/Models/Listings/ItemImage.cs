using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Room2Room.Models.Listings;

[Table("ItemImage")]
public class ItemImage
{
    public int Id { get; set; }
    [Required]
    public string ImagePath { get; set; }
    [Required]
    public int ItemId { get; set; } // foreign key to Items
    public Item Item { get; set; } // many to one relationship with Items
}