using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Room2Room.Models.Listings;


[Table("Item")] // This attribute specifies the name of the table in the database that this class maps to
public class Item
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
    public int AccountId { get; set; }
    public Category Category { get; set; }
    public List<OrderDetail> OrderDetail { get; set; }
    public List<CartDetail> CartDetail { get; set; }
    public List<ItemImage> ItemImage { get; set; } // one to many relationship with ItemImage

    [NotMapped]
    public string CategoryName { get; set; }
    
    [NotMapped]
    public string ImagePath { get; set; }

}