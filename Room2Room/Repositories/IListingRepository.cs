namespace Room2Room
{
    public interface IListingRepository
    {
        Task<IEnumerable<Item>> GetItems(string sTerm="", int? categoryId=null, int? universityId=null);
        Task<IEnumerable<Category>> GetCategories();
        Task AddItemAsync(Item newItem);
        Task AddItemImageAsync(ItemImage image);
        Task<Item?> GetItemById(int id);
        Task Delete(int id);
        Task<List<int>> GetWatchlistedItemIdsAsync(int userId);
        Task<string> GetUniversityNameByAccountIdAsync(int accountId);
    }
}