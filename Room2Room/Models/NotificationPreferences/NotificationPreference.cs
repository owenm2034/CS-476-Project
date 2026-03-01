using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models.NotificationPreferences;

public class NotificationPreference
{
    public NotificationPreference() {}

    public NotificationPreference(int accountId)
    {
        AccountId = accountId;
    }

    [Key]
    public int AccountId { get; set; }
    public bool RecieveEmailNotificationOnChatMessageRecieved { get; set; } = true;
    public bool RecieveEmailNotificationOnUserReported { get; set; } = true;
    public bool RecieveEmailNotificationOnListingReported { get; set; } = true;
}
