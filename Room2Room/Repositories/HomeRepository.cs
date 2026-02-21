using Microsoft.EntityFrameworkCore;

namespace Room2Room.Repositories;
public class HomeRepository : IHomeRepository
{
    private readonly ApplicationDbContext _db; //declaring field of type ApplicationDbContext, which is used to interact with the database in the repository
    public HomeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Category>> GetCategories() // This method retrieves a list of categories from the database. It uses Entity Framework Core's ToListAsync method to asynchronously fetch all categories from the Categories table and return them as an IEnumerable of Category objects.
    {
        return await _db.Categories.ToListAsync();
    }


    public async Task<IEnumerable<Item>> GetItems(string sTerm="", int categoryId=0) //expecting a search term and category id as parameters, which are used to filter the items returned from the database
    {
       sTerm = sTerm.ToLower(); 
       IEnumerable<Item> items = await (from item in _db.Items      // LINQ query to retrieve items from the database, joining with the Categories table to get the category name for each item
                    join category in _db.Categories
                    on item.CategoryId equals category.Id
                    where string.IsNullOrWhiteSpace(sTerm) || (item!=null && item.ItemName.ToLower().Contains(sTerm)) // The where clause filters the items based on the search term (sTerm) and categoryId. If sTerm is null or whitespace, it returns all items. Otherwise, it checks if the item name contains the search term (case-insensitive). It also checks if the categoryId is 0 (which means all categories) or if the item's CategoryId matches the provided categoryId.
                    where categoryId == 0 || item.CategoryId == categoryId
                    select new Item
                    {
                        Id = item.Id,
                        ItemName = item.ItemName,
                        ItemDescription = item.ItemDescription,
                        ItemPrice = item.ItemPrice,
                        Status = item.Status,
                        CategoryId = item.CategoryId,
                        AccountId = item.AccountId,
                        CategoryName = category.CategoryName,

                        ImagePath = _db.ItemImages
                                        .Where(img => img.ItemId == item.Id)
                                        .Select(img => img.ImagePath)
                                        .FirstOrDefault()
                    }
                    ).ToListAsync();
       //return await items;
       if (categoryId > 0) // If a specific category is selected (categoryId > 0), filter the items to include only those that belong to the selected category. This is done using the Where method, which checks if the CategoryId of each item matches the provided categoryId.
       {
            items = items.Where(a => a.CategoryId == categoryId).ToList();
        }
         return items;
    }


    public async Task AddItemAsync(Item newItem) // This method adds a new item to the database. It takes an Item object as a parameter, adds it to the Items DbSet, and then calls SaveChangesAsync to persist the changes to the database.
    {
        if (newItem == null)
        {
            throw new ArgumentNullException(nameof(newItem));
        }
        _db.Items.Add(newItem);
        await _db.SaveChangesAsync();
    }

    public async Task AddItemImageAsync(ItemImage image)
{
    if (image == null) throw new ArgumentNullException(nameof(image));

    _db.ItemImages.Add(image);
    await _db.SaveChangesAsync();
}
}

