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
        string profilePictureUrl,
        int universityId
    )
    {
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Username = username;
        ProfilePictureUrl = profilePictureUrl;
        UniversityId = universityId;
    }

    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string Username { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
    public string ProfilePictureUrl { get; set; }
    public int UniversityId { get; set; }
    public virtual NotificationPreference? NotificationPreferences { get; set; }
}
