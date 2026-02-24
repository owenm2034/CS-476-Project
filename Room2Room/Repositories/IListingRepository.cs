namespace Room2Room
{
    public interface IListingRepository
    {
        Task<IEnumerable<Item>> GetItems(string sTerm="", int categoryId=0, int? universityId=null);
        Task<IEnumerable<Item>> GetAllItemsForAdmin();
        Task<IEnumerable<Category>> GetCategories();
        Task AddItemAsync(Item newItem);
        Task AddItemImageAsync(ItemImage image);
        Task<Item?> GetItemById(int id);
        Task Delete(int id);
    }
}