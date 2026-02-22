namespace Room2Room
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Item>> GetItems(string sTerm="", int categoryId=0, int? universityId=null);
        Task<IEnumerable<Item>> GetAllItemsForAdmin();
        Task<IEnumerable<Category>> GetCategories();
        Task AddItemAsync(Item newItem);
        Task AddItemImageAsync(ItemImage image);
    }
}