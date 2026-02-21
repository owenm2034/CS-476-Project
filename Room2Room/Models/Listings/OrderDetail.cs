using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Room2Room.Models.Listings;

[Table("OrderDetail")]
public class OrderDetail
{
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    
    public int ItemId { get; set; }
    public Order Order { get; set; }
    public Item Item { get; set; }    
}