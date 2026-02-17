namespace Room2Room.Models.NotificationPreferences;

public class AdministratorNotificationPreference : NotificationPreference
{
    public bool RecieveEmailNotificationOnUserReported { get; set; } = true;
    public bool RecieveEmailNotificationOnListingReported { get; set; } = true;
}
