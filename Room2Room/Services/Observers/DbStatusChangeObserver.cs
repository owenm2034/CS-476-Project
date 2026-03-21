using Room2Room.Models.UserNotification;
namespace Room2Room.Services.Observers
{
    public class DbStatusChangeObserver : IItemObserver
    {
        private readonly ApplicationDbContext _context;

        public DbStatusChangeObserver(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Update(IItemSubject subject)
        {
            var service = subject as ItemSubject;
            if (service?.OldItem == null || service.NewItem == null) return;

            if (service.NewItem.Status == service.OldItem.Status) return;

            //Console.WriteLine($"StatusChangeObserver: verified the status was changed and executed.");
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
                    Type = "StatusChange",
                    Message = $"Status change alert: The {service.NewItem.ItemName} you saved on your watchlist has changed from {service.OldItem.Status} to {service.NewItem.Status}."
                });
            }
            _context.SaveChanges();
        }
    }
            
}