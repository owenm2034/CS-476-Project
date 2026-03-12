using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models.Chats;

public class ChatMessage
{
    public ChatMessage()
    {

    }

    public ChatMessage(SendChatModel scm)
    { 
        this.Message = scm.Message;
        if (scm.ChatId.HasValue)
        {
            this.ChatId = scm.ChatId.Value;
        }
    }

    [Key]
    public int MessageId { get; set; }
    public int ChatId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FromAccountId { get; set; }
}