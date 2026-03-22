using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Room2Room.Models.Listings;

public class ListingDetail
{
    public Item Listing { get; set; }
    public string SellerName { get; set; } = "Unknown";
    public string SellerEmail { get; set; } = "";
    public List<ItemImage> AllImages { get; set; } = new();
}