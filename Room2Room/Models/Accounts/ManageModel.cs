using Room2Room.Models.NotificationPreferences;

namespace Room2Room.Models.Accounts;

public class ManageModel
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? OldPassword { get; set; }
    public string? Password { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string SuccessMessage { get; set; } = "";
    public NotificationPreference NotificationPreferences { get; set; }
}