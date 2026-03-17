namespace Room2Room.Models.Chats;

public class AllChatsModel
{
    public List<ChatModel> ChatModels { get; set; }
    public int SelectedIndex { get; set; } = 0;
    // for when admins view chat, it is the account id of the messages to show in blue
    public int? ViewingId { get; set; }
}