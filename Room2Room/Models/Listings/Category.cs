using System.ComponentModel.DataAnnotations;
namespace Room2Room.Models.Listings;
public class Category
{
    public int Id { get; set; }
    [Required]
    [MaxLength(40)]
    public string CategoryName { get; set; }

    public List<Item> Item { get; set; } // one category can have many items
   
    
}