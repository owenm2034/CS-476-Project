using System.ComponentModel.DataAnnotations.Schema;

namespace Room2Room.Models.Chats;

public class PrivateChat : Chat
{
    public PrivateChat()
    {
        ChatType = "private";
    }

    [NotMapped]
    public List<int> AccountIds { get; set; }

    public override void SetTarget(List<int> targetIds)
    {
        AccountIds = targetIds;
    }
}