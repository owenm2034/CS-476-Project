using System.ComponentModel.DataAnnotations;
namespace Room2Room.Models.Listings;
public  class Order
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Required]
    public int OrderStatusId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public OrderStatus OrderStatus { get; set; }
    public List<OrderDetail> OrderDetail { get; set; }
}