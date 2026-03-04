namespace Room2Room.Models.DTOs
{
    public class ItemDisplayModel
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<University> Universities { get; set; }
        
        public string STerm { get; set; } = "";
        public int? CategoryId { get; set; }
        public int? UniversityId { get; set; }
        
    }

}