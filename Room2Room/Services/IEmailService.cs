public interface IEmailService
{
   Task SendWelcomeEmailAsync(string toEmail, string username);
   Task SendPriceDropEmailAsync(string toEmail, string username, string itemName, double oldPrice, double newPrice);
   Task SendStatusChangeEmailAsync(string toEmail, string username, string itemName, string oldStatus, string newStatus);

}