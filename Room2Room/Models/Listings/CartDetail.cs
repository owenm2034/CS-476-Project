using System.ComponentModel.DataAnnotations;
namespace Room2Room.Models.Listings;
public class CartDetail
{
    public int Id { get; set; }
    [Required]
    public int ShoppingCartId { get; set; }
    [Required]
    public int ItemId { get; set; }
    public Item Item { get; set; }
    public ShoppingCart ShoppingCart { get; set; }

}