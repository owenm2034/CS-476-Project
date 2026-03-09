namespace Room2Room.Models.DTOs
{
    public class EditItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = "";
        public string ItemDescription { get; set; } = "";
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string? ReturnTo { get; set; }
    }
}