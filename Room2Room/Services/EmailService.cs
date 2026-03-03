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
            to = new[] { "pushpmehrok.singh@gmail.com" }, // Will change toEmail after domain verification
            subject = "Welcome to Room2Room!",
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
                        <h1 style=""margin:0;font-size:32px;font-weight:bold;color:#000;"">
                            ACCOUNT ACTIVATED
                        </h1>
                        </td>
                    </tr>

                    <tr>
                        <td style=""padding:0 40px 30px;font-size:15px;color:#222;"">
                        <p style=""margin:0 0 15px;font-size:20px;font-weight:bold;"">
                            Hi {username},
                        </p>
                        <p style=""margin:0 0 25px;line-height:1.5;"">
                            Congrats, you've activated your Room2Room account. Next time you shop around, we recommend logging in for a smoother experience.
                        </p>

                        <div style=""text-align:center;"">
                            <a href=""https://yoursite.com/login""
                            style=""background:#222;color:#ffffff;
                                    text-decoration:none;
                                    padding:14px 28px;
                                    border-radius:40px;
                                    font-weight:bold;
                                    display:inline-block;"">
                            START SHOPPING
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
        var responseBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
    }
}