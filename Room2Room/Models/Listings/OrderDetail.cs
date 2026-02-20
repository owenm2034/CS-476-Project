using System.ComponentModel.DataAnnotations;
namespace Room2Room.Models.Listings;
public class OrderDetail
{
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    
    public int ItemId { get; set; }
    public Order Order { get; set; }
    public Item Item { get; set; }    
}