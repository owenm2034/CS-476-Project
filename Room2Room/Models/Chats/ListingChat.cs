
namespace Room2Room.Models.Chats;

public class ListingChat : Chat
{
    public int ListingId { get; set; }

    public override void SetTarget(List<int> targetIds)
    {
        ListingId = targetIds.FirstOrDefault();
    }
}