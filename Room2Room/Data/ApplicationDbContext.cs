using Microsoft.EntityFrameworkCore;
using Room2Room.Models;
using Room2Room.Models.Accounts;
using Room2Room.Models.Listings;
using Room2Room.Models.NotificationPreferences;
using Room2Room.Models.Announcements;


namespace Room2Room.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Announcement> Announcements { get; set; }

    public DbSet<Item> Items { get; set; } // the label "Items" will be discarded and replaced with "Item" as the table name in the database due to the [Table("Item")] attribute in the Item class
    public DbSet<ItemImage> ItemImages { get; set; } 
    public DbSet<Category> Categories { get; set; }
    public DbSet<SaveLater> SaveLaters { get; set; }
    public DbSet<SaveLaterDetail> SaveLaterDetails { get; set; }
    
}
