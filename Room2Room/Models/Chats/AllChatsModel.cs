namespace Room2Room.Models.Chats;

public class AllChatsModel
{
    public List<ChatModel> ChatModels { get; set; }
    public int SelectedIndex { get; set; } = 0;
}