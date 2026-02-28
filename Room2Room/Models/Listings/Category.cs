using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Room2Room.Models.Listings;

[Table("Category")]
public class Category
{
    public int Id { get; set; }
    [Required]
    [MaxLength(40)]
    public string CategoryName { get; set; }

    public List<Item> Item { get; set; } // one category can have many items
   
    
}