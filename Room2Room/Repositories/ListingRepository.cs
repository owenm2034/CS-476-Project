using Microsoft.EntityFrameworkCore;
using Room2Room.Models;

namespace Room2Room.Repositories;

public class ListingRepository : IListingRepository
{
    private readonly ApplicationDbContext _db; //declaring field of type ApplicationDbContext, which is used to interact with the database in the repository

    public ListingRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Category>> GetCategories() // This method retrieves a list of categories from the database. It uses Entity Framework Core's ToListAsync method to asynchronously fetch all categories from the Categories table and return them as an IEnumerable of Category objects.
    {
        return await _db.Categories.ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetItems(
        string sTerm = "",
        int? categoryId = null,
        int? universityId = null
    ) //expecting a search term and category id as parameters, which are used to filter the items returned from the database
    {
        IEnumerable<Item> items = await (
            from item in _db.Items // LINQ query to retrieve items from the database, joining with the Categories table to get the category name for each item
            join category in _db.Categories on item.CategoryId equals category.Id
            join account in _db.Accounts on item.AccountId equals account.Id
            join university in _db.Universities on account.UniversityId equals university.Id
            where
                string.IsNullOrWhiteSpace(sTerm)
                || (item.ItemName != null && item.ItemName.ToLower().Contains(sTerm.ToLower()))
            where categoryId == null || item.CategoryId == categoryId
            where universityId == null || account.UniversityId == universityId
            select new Item
            {
                Id = item.Id,
                ItemName = item.ItemName,
                ItemDescription = item.ItemDescription,
                ItemPrice = item.ItemPrice,
                Status = item.Status,
                CategoryId = item.CategoryId,
                AccountId = item.AccountId,
                UniversityName = university.Name,
                CategoryName = category.CategoryName,
                ImagePath = _db.ItemImages
                    .Where(img => img.ItemId == item.Id)
                    .Select(img => img.ImagePath)
                    .FirstOrDefault() ?? ""
            }
        ).ToListAsync();
        //return await items;
        if (categoryId != null) // If a specific category is selected (categoryId > 0), filter the items to include only those that belong to the selected category. This is done using the Where method, which checks if the CategoryId of each item matches the provided categoryId.
        {
            items = items.Where(a => a.CategoryId == categoryId).ToList();
        }

        return items;
    }

    public async Task<IEnumerable<Item>> GetItemsByAccountId(int accountId)
    {
        IEnumerable<Item> items = await (
            from item in _db.Items
            join category in _db.Categories on item.CategoryId equals category.Id
            join account in _db.Accounts on item.AccountId equals account.Id
            join university in _db.Universities on account.UniversityId equals university.Id
            where item.AccountId == accountId
            select new Item
            {
                Id = item.Id,
                ItemName = item.ItemName,
                ItemDescription = item.ItemDescription,
                ItemPrice = item.ItemPrice,
                Status = item.Status,
                CategoryId = item.CategoryId,
                AccountId = item.AccountId,
                UniversityName = university.Name,
                CategoryName = category.CategoryName,
                ImagePath = _db.ItemImages
                    .Where(img => img.ItemId == item.Id)
                    .Select(img => img.ImagePath)
                    .FirstOrDefault() ?? ""
            }
        ).ToListAsync();

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
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        _db.ItemImages.Add(image);
        await _db.SaveChangesAsync();
    }

    public async Task<Item?> GetItemById(int id)
    {
        return await _db.Items.FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task UpdateItemAsync(Item item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _db.Items.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        await _db.Items.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public async Task<List<int>> GetWatchlistedItemIdsAsync(int userId)
    {
        return await _db.Watchlists
        .Where(w => w.UserId == userId)
        .Select(w => w.ItemId)
        .ToListAsync();
    }

    public async Task<string> GetUniversityNameByAccountIdAsync(int accountId)
    {
        var result = await (
            from account in _db.Accounts
            join university in _db.Universities on account.UniversityId equals university.Id
            where account.Id == accountId
            select university.Name
        ).FirstOrDefaultAsync();

        return result ?? "";
    }
}