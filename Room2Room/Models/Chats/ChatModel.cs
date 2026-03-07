namespace Room2Room.Models.Chats;

public class ChatModel
{
    public Chat Chat { get; set; }
    public Item? Item { get; set; }
    public Dictionary<int, string> AccountIdToNameDictionary { get; set; }
    public List<ChatMessage> Messages { get; set; }

    public string ChatName
    {
        get
        {
            if (Item != null)
            {
                return Item.ItemName;
            }
            else
                return string.Join(", ", AccountIdToNameDictionary.Select(x => x.Value));
        }
    }

    public DateTime? LastUpdated
    {
        get { return Messages.OrderByDescending(x => x.CreatedAt).FirstOrDefault()?.CreatedAt; }
    }
}
