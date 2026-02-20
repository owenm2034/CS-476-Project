using System.ComponentModel.DataAnnotations;
namespace Room2Room.Models.Listings;
public class ShoppingCart
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    public bool IsDeleted { get; set; } = false;
}