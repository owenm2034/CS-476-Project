using System.ComponentModel.DataAnnotations.Schema;
using Room2Room.Models.NotificationPreferences;

namespace Room2Room.Models.Accounts;

public class Account
{
    // public Account() { }

    public Account(
        string email,
        string passwordHash,
        string passwordSalt,
        string username,
        string profilePictureUrl
    )
    {
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Username = username;
        ProfilePictureUrl = profilePictureUrl;
    }

    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string Username { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
    public string ProfilePictureUrl { get; set; }

    [NotMapped]
    public NotificationPreference? NotificationPreferences { get; set; }
}
