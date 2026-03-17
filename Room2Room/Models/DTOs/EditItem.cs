namespace Room2Room.Models.DTOs
{
    public class EditItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = "";
        public string ItemDescription { get; set; } = "";
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; } = "Available";
        public string? ReturnTo { get; set; }
        public string? CurrentImagePath { get; set; }
        public IFormFile? NewImage { get; set; }
    }
}