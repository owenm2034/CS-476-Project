namespace Room2Room
{
    public interface IListingRepository
    {
        Task<IEnumerable<Item>> GetItems(string sTerm="", int categoryId=0); // This method is defined in the IHomeRepository interface, which is implemented by the HomeRepository class. It takes two optional parameters: sTerm (a search term) and categoryId (an integer representing the category ID). The method returns a Task that resolves to an IEnumerable of Item objects, which are filtered based on the search term and category ID provided.
        Task<IEnumerable<Category>> GetCategories();
        Task AddItemAsync(Item newItem);
        Task AddItemImageAsync(ItemImage image);
        Task<Item?> GetItemById(int id);
        Task Delete(int id);
    }
}