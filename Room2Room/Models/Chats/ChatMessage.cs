using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models.Chats;

public class ChatMessage
{
    [Key]
    public int MessageId { get; set; }
    public int ChatId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FromAccountId { get; set; }
}