public interface IEmailService
{
   Task SendWelcomeEmailAsync(string toEmail, string username);
}