namespace Room2Room
{
    public interface IListingRepository
    {
        Task<IEnumerable<Item>> GetItems(string sTerm="", int? categoryId=null, int? universityId=null);
        Task<IEnumerable<Category>> GetCategories();
        Task AddItemAsync(Item newItem);
        Task AddItemImageAsync(ItemImage image);
        Task<Item?> GetItemById(int id);
        Task UpdateItemAsync(Item item);
        Task Delete(int id);
        Task<List<int>> GetWatchlistedItemIdsAsync(int userId);
        Task<string> GetUniversityNameByAccountIdAsync(int accountId);
        Task<IEnumerable<Item>> GetItemsByAccountId(int accountId);
        Task<ItemImage?> GetFirstItemImageAsync(int itemId);
        Task UpdateItemImageAsync(ItemImage image);
    }
}