using Microsoft.EntityFrameworkCore;
using Room2Room.Models;
using Room2Room.Models.Accounts;
using Room2Room.Models.NotificationPreferences;

namespace Room2Room.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }
    public DbSet<University> Universities { get; set; }
}
