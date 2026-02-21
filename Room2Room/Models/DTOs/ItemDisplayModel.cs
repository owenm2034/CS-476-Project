namespace Room2Room.Models.DTOs
{
    public class ItemDisplayModel
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }

}