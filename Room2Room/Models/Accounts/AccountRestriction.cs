using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models.Accounts;

public class AccountRestriction
{
    [Key]
    public int AccountId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}