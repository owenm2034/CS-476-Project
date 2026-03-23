namespace Room2Room.Models.DTOs;

public class ProfileViewModel
{
    public string Username { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public List<WatchListDisplayModel> SavedListings { get; set; } = [];
    public List<ProfileChatPreviewModel> ActiveChats { get; set; } = [];
}

public class ProfileChatPreviewModel
{
    public int ChatId { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public string LastMessagePreview { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public bool IsListingChat { get; set; }
}
