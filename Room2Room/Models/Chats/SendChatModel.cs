namespace Room2Room.Models.Chats;

public class SendChatModel
{
    public int? ChatId { get; set; }
    public int? ListingId { get; set; }
    public int? ToAccountId { get; set; }
    public string Message { get; set; }
}
