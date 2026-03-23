using System.ComponentModel.DataAnnotations.Schema;

namespace Room2Room.Models.Accounts;

[Table("UserReport")]
public class UserReport
{
    public int Id { get; set; }
    public int ReportedAccountId { get; set; }
    public int ReportedByAccountId { get; set; }
    public string Reason { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Resolution { get; set; } = null;
    public DateTime? ResolvedAt { get; set; } = null;

    [NotMapped]
    public Account? ReportedAccount { get; set; }
    [NotMapped]
    public Account? Reporter { get; set; }
}