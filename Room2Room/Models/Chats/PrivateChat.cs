namespace Room2Room.Models.Chats;

public class PrivateChat : Chat
{
    public List<int> AccountIds { get; set; }

    public override void SetTarget(List<int> targetIds)
    {
        AccountIds = targetIds;
    }
}