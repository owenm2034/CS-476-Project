namespace Room2Room.Services.Observers
{
    public class EmailPriceDropObserver : IItemObserver
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public EmailPriceDropObserver (ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public void Update(IItemSubject subject)
        {
            var service = subject as ItemSubject;
            if (service?.OldItem == null || service.NewItem == null) return; // Ensure we have valid items to compare

            if (service.NewItem.ItemPrice >= service.OldItem.ItemPrice) return; // check and process only if there is a price drop

            //Console.WriteLine($"EmailPriceDropObserver: verified the price was dropped, sending emails."); ------works
            
            var watchers = _context.Watchlists
                .Where(w => w.ItemId == service.NewItem.Id)
                .Join(_context.Accounts,
                    w => w.UserId,
                    a => a.Id,
                    (w, a) => new {a.Email, a.Username})
                .ToList();
            
            foreach (var watcher in watchers)
            {
                // send and forget
                _ = _emailService.SendPriceDropEmailAsync(
                    watcher.Email,
                    watcher.Username,
                    service.NewItem.ItemName,
                    service.OldItem.ItemPrice,
                    service.NewItem.ItemPrice);
            }
        }

    }
}