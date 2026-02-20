using System.ComponentModel.DataAnnotations;
namespace Room2Room.Models.Listings;
public class OrderStatus
{
    public int Id { get; set; }
    [Required, MaxLength(20)]
    public string? StatusName { get; set; }
}