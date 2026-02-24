using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
namespace Room2Room.Models.Listings;

[Table("SaveLater")]
public class SaveLater
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    public bool IsDeleted { get; set; } = false;
}