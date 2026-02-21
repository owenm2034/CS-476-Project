namespace Room2Room.Models.DTOs
{
    public class CreateItem
    {
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}