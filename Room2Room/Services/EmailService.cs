using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Room2Room.Models;

namespace Room2Room.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly HttpClient _httpClient;

    public EmailService(IOptions<EmailSettings> settings, IHttpClientFactory httpClientFactory)
    {
        _settings = settings.Value;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string username)
    {
        var payload = new
        {
            from = $"{_settings.FromName} <{_settings.FromEmail}>",
            to = new[] { toEmail },
            subject = "Welcome to Room2Room!",
            html = $@"
                <h2>Congrats, you've activated your Room2Room account.</h2>

                <div style=""text-align:center;"">
                    <a href=""https://www.room2room.xyz/Account/Login""
                    style=""background:#222;color:#ffffff;
                            text-decoration:none;
                            padding:14px 28px;
                            border-radius:40px;
                            font-weight:bold;
                            display:inline-block;"">
                    Log In
                    </a>
                </div>
                "
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
    }

    public async Task SendPriceDropEmailAsync(string toEmail, string username, string itemName, double oldPrice, double newPrice)
    {
        var payload = new
        {
            from = $"{_settings.FromName} <{_settings.FromEmail}>",
            to = new[] { toEmail },
            subject = $"Price Drop Alert: {itemName}",
            html = $@"
            <div style=""margin:0;padding:0;background:#f4f4f4;"">
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                <td align=""center"">
                    <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
                        style=""background:#ffffff;font-family:Arial,Helvetica,sans-serif;"">

                    <tr>
                        <td align=""center"" style=""padding:30px 20px 10px;"">
                        <h2 style=""margin:0;font-size:24px;font-weight:bold;letter-spacing:1px;"">
                            Room2Room
                        </h2>
                        </td>
                    </tr>

                    <tr>
                        <td style=""padding:0 40px 30px;font-size:15px;color:#222;"">
                        <p style=""margin:0 0 15px;font-size:20px;font-weight:bold;"">Hi {username},</p>
                        <p style=""margin:0 0 25px;line-height:1.5;"">
                            Good news! An item on your watchlist has dropped in price.
                        </p>
                        <p style=""margin:0 0 10px;""><strong>Item:</strong> {itemName}</p>
                        <p style=""margin:0 0 10px;""><strong>Old Price:</strong> <span style=""text-decoration:line-through;color:#999;"">${oldPrice}</span></p>
                        <p style=""margin:0 0 25px;""><strong>New Price:</strong> <span style=""color:green;font-weight:bold;"">${newPrice}</span></p>
                        <div style=""text-align:center;"">
                            <a href=""https://Room2Room.com/Watchlist""
                            style=""background:#222;color:#ffffff;
                                    text-decoration:none;
                                    padding:14px 28px;
                                    border-radius:40px;
                                    font-weight:bold;
                                    display:inline-block;"">
                            My Watchlist
                            </a>
                        </div>
                        </td>
                    </tr>

                    <tr>
                        <td align=""center"" style=""padding:30px 20px;font-size:12px;color:#777;"">
                        © 2026 Room2Room. All rights reserved.
                        </td>
                    </tr>

                    </table>
                </td>
                </tr>
            </table>
            </div>"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
        response.EnsureSuccessStatusCode();

    }

    public async Task SendStatusChangeEmailAsync(string toEmail, string username, string itemName, string oldStatus, string newStatus)
    {
        var payload = new
        {
            from = $"{_settings.FromName} <{_settings.FromEmail}>",
            to = new[] { toEmail },
            subject = $"Status Change: {itemName}",
            html = $@"
            <div style=""margin:0;padding:0;background:#f4f4f4;"">
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                <td align=""center"">
                    <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
                        style=""background:#ffffff;font-family:Arial,Helvetica,sans-serif;"">

                    <tr>
                        <td align=""center"" style=""padding:30px 20px 10px;"">
                        <h2 style=""margin:0;font-size:24px;font-weight:bold;letter-spacing:1px;"">
                            Room2Room
                        </h2>
                        </td>
                    </tr>

                    <tr>
                        <td style=""padding:0 40px 30px;font-size:15px;color:#222;"">
                        <p style=""margin:0 0 15px;font-size:20px;font-weight:bold;"">Hi {username},</p>
                        <p style=""margin:0 0 25px;line-height:1.5;"">
                            An item on your watchlist has changed it's status. Please review your watchlist to see the updated availability of the item.
                        </p>
                        <p style=""margin:0 0 10px;""><strong>Item:</strong> {itemName}</p>
                        <p style=""margin:0 0 10px;""><strong>Old Price:</strong> <span style=""text-decoration:line-through;color:#999;"">${oldStatus}</span></p>
                        <p style=""margin:0 0 25px;""><strong>New Price:</strong> <span style=""color:green;font-weight:bold;"">${newStatus}</span></p>
                        <div style=""text-align:center;"">
                            <a href=""https://Room2Room.com/Watchlist""
                            style=""background:#222;color:#ffffff;
                                    text-decoration:none;
                                    padding:14px 28px;
                                    border-radius:40px;
                                    font-weight:bold;
                                    display:inline-block;"">
                            My Watchlist
                            </a>
                        </div>
                        </td>
                    </tr>

                    <tr>
                        <td align=""center"" style=""padding:30px 20px;font-size:12px;color:#777;"">
                        © 2026 Room2Room. All rights reserved.
                        </td>
                    </tr>

                    </table>
                </td>
                </tr>
            </table>
            </div>"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
        response.EnsureSuccessStatusCode();

    }
public async Task SendUserReportEmailAsync(string toEmail, string reportedUsername, string reporterUsername, string reason)
{
    var payload = new
    {
        from = $"{_settings.FromName} <{_settings.FromEmail}>",
        to = new[] { toEmail },
        subject = $"User Reported: {reportedUsername}",
        html = $@"
        <div style=""margin:0;padding:0;background:#f4f4f4;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
            <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
                    style=""background:#ffffff;font-family:Arial,Helvetica,sans-serif;"">

                <tr>
                    <td align=""center"" style=""padding:30px 20px 10px;"">
                    <h2 style=""margin:0;font-size:24px;font-weight:bold;letter-spacing:1px;"">
                        Room2Room
                    </h2>
                    </td>
                </tr>

                <tr>
                    <td align=""center"" style=""padding:10px 20px 30px;"">
                    <h1 style=""margin:0;font-size:28px;font-weight:bold;color:#c0392b;"">
                        USER REPORTED
                    </h1>
                    </td>
                </tr>

                <tr>
                    <td style=""padding:0 40px 30px;font-size:15px;color:#222;"">
                    <p style=""margin:0 0 15px;""><strong>Reported User:</strong> {reportedUsername}</p>
                    <p style=""margin:0 0 15px;""><strong>Reported By:</strong> {reporterUsername}</p>
                    <p style=""margin:0 0 15px;""><strong>Reason:</strong> {reason}</p>
                    <p style=""margin:0 0 25px;color:#777;font-size:13px;"">
                        Please review this report in the admin panel.
                    </p>
                    <div style=""text-align:center;"">
                        <a href=""https://yoursite.com/Admin""
                        style=""background:#c0392b;color:#ffffff;
                                text-decoration:none;
                                padding:14px 28px;
                                border-radius:40px;
                                font-weight:bold;
                                display:inline-block;"">
                        View Admin Panel
                        </a>
                    </div>
                    </td>
                </tr>

                <tr>
                    <td align=""center"" style=""padding:30px 20px;font-size:12px;color:#777;"">
                    © 2026 Room2Room. All rights reserved.
                    </td>
                </tr>

                </table>
            </td>
            </tr>
        </table>
        </div>"
    };

    var json = JsonSerializer.Serialize(payload);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
    response.EnsureSuccessStatusCode();
}
}
