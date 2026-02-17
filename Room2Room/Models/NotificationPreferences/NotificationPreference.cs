using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models.NotificationPreferences;

public class NotificationPreference
{
    [Key]
    public int AccountId { get; set; }
    public bool RecieveEmailNotificationOnChatMessageRecieved { get; set; } = true;
}