using Room2Room.Models.UserNotification;

namespace Room2Room.Services.Observers
{
    public class DbPriceDropObserver : IItemObserver
    {
        private readonly ApplicationDbContext _context;

        public DbPriceDropObserver(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Update(IItemSubject subject)
        {
            var service = subject as ItemSubject;
            if (service?.OldItem == null || service.NewItem == null) return; // Ensure we have valid items to compare

            if (service.NewItem.ItemPrice >= service.OldItem.ItemPrice) return; // check and process only if there is a price drop

            //Console.WriteLine($"PriceDropObserver: verified the price was dropped and executed.");
            var watchers = _context.Watchlists
                .Where(w => w.ItemId == service.NewItem.Id)
                .Select(w => w.UserId)
                .ToList();

            foreach (var userId in watchers)
            {
                _context.UserNotifications.Add(new UserNotification
                {
                    UserId = userId,
                    ItemId = service.NewItem.Id,
                    Type = "PriceDrop",
                    Message = $"Price drop alert: The {service.NewItem.ItemName} you saved on your watchlist is now ${service.NewItem.ItemPrice} (previously it was ${service.OldItem.ItemPrice})"
                });
            }
            _context.SaveChanges();
        }
    }
            
}