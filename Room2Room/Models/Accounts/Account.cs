using Room2Room.Models.NotificationPreferences;

namespace Room2Room.Models.Accounts;

public class Account
{
    public virtual int Id { get; set; }
    public virtual string Email { get; set; }
    public virtual string PasswordHash { get; set; }
    public virtual string Username { get; set; }
    public virtual bool IsEmailVerified { get; set; }
    public virtual bool IsAdmin { get; set; } = false;
    public virtual string ProfilePictureUrl { get; set; }
    public virtual NotificationPreference? NotificationPreferences { get; set; }
}
