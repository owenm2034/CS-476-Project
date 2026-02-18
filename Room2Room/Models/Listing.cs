using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models;

public class Listing
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = null!;
    [Required]
    public string Category {get; set;} = null!;
    [Required]
    public string Condition {get; set;} = null!;
    [Required]
    public string Status {get; set;} = "Available";
    [Required]
    [Range(1, 10000, ErrorMessage = "Please post for items worth at least CAD $1 and less than CAD $10000")]
    public decimal Price {get; set; }
    public  DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;

}