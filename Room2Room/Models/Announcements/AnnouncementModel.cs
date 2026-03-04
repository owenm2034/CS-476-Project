namespace Room2Room.Models.Announcements;

public class AnnouncementFormModel
{
    public int Id { get; set; } // used for Edit
    public string? Message { get; set; }
    public DateTime? StartDate { get; set; } // optional
    public DateTime? EndDate { get; set; } // optional
}
