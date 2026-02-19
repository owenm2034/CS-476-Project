namespace Room2Room.Models.Announcements;

public class Announcement
{
    public Announcement(
        string message,
        string color,
        DateTime startDate,
        DateTime endDate
    )
    {
        Message = message;
        Color = color;
        StartDate = startDate;
        EndDate = endDate;
    }

    public int Id { get; set; }
    public string Message { get; set; }
    public string Color { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
