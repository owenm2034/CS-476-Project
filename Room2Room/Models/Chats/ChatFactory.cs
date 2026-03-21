namespace Room2Room.Models.Chats;

public static class ChatFactory
{
    public static Chat CreateChat(string type)
    {
        switch (type)
        {
            case "private":
                return new PrivateChat();
            case "listing":
                return new ListingChat();
            default:
                throw new ArgumentException($"Invalid type: {type}");
        }
    }
}