using Microsoft.EntityFrameworkCore;

namespace Room2Room.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    
}
