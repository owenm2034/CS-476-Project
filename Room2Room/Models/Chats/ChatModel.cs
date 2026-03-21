namespace Room2Room.Models.Chats;

public class ChatModel
{
    public Chat Chat { get; set; }
    public string ChatName { get; set; }
    public Dictionary<int, string> AccountIdToNameDictionary { get; set; }
    public List<ChatMessage> Messages { get; set; }
    public int? ViewingId { get; set; }

    public DateTime? LastUpdated
    {
        get { return Messages.OrderByDescending(x => x.CreatedAt).FirstOrDefault()?.CreatedAt; }
    }
}
