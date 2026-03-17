namespace Room2Room.Models.DTOs;

public class AdminUsersViewModel
{
    public IEnumerable<AdminUserListItem> Users { get; set; } = new List<AdminUserListItem>();
    public string STerm { get; set; } = string.Empty;
}

public class AdminUserListItem
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public string UniversityName { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = "Active";
}

public class AdminEditUserModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}