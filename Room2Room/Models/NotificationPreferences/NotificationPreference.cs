namespace Room2Room.Models.NotificationPreferences;

public class NotificationPreference
{
    public int AccountId { get; set; }
    public bool RecieveEmailNotificationOnChatMessageRecieved { get; set; } = true;
}