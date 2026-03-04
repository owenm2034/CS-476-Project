using Microsoft.EntityFrameworkCore;
using Room2Room.Models;
using Room2Room.Models.Accounts;
using Room2Room.Models.Listings;
using Room2Room.Models.NotificationPreferences;
using Room2Room.Models.Announcements;
using Room2Room.Models.Watchlist;


namespace Room2Room.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Announcement> Announcements { get; set; }

    public DbSet<Item> Items { get; set; } 
    public DbSet<ItemImage> ItemImages { get; set; } 
    public DbSet<Category> Categories { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    
}
