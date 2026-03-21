namespace Room2Room.Services.Observers
{
    public class EmailStatusChangeObserver : IItemObserver
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public EmailStatusChangeObserver (ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public void Update(IItemSubject subject)
        {
            var service = subject as ItemSubject;
            if (service?.OldItem == null || service.NewItem == null) return; // Ensure we have valid items to compare

            if (service.NewItem.Status == service.OldItem.Status) return; // check and process only if there is a change in status

            //Console.WriteLine($"EmailStatusChangeObserver: verified the status was changed, sending emails."); --- works
            
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
                _ = _emailService.SendStatusChangeEmailAsync(
                    watcher.Email,
                    watcher.Username,
                    service.NewItem.ItemName,
                    service.OldItem.Status ?? "Unknown",
                    service.NewItem.Status ?? "Unknown");
            }
        }

    }
}