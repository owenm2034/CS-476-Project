namespace Room2Room.Models.Chats;

public class ChatModel
{
    public Chat Chat { get; set; }
    public Item? Item { get; set; }
    public Dictionary<int, string> AccountIdToNameDictionary { get; set; }
    public List<ChatMessage> Messages { get; set; }
}
