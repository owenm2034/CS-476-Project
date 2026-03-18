using System.ComponentModel.DataAnnotations.Schema;

namespace Room2Room.Models.Chats;

public abstract class Chat
{
    public int ChatId { get; set; }
    public string ChatType { get; set; }
    abstract public void SetTarget(List<int> targetIds);
}