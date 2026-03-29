using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models.DTOs
{
    public class CreateItem
    {
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        [Range(typeof(double), "0", "10000")]
        public double Price { get; set; }
        public int CategoryId { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}