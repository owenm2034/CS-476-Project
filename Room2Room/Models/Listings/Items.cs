using System.ComponentModel.DataAnnotations;
namespace Room2Room.Models.Listings;
public class Items
{
    public int Id { get; set; }
    [Required]
    [MaxLength(40)]
    public string? ItemName { get; set; }
    [MaxLength(200)]
    public string ItemDescription { get; set; }
    [Required]
    public double ItemPrice { get; set; }
    [Required]
    public string Status { get; set; } = "Available";
    [Required]
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<OrderDetail> OrderDetail { get; set; }
    public List<CartDetail> CartDetail { get; set; }
    public List<ItemImage> ItemImages { get; set; } // one to many relationship with ItemImage
}