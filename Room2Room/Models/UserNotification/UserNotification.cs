using System.ComponentModel.DataAnnotations.Schema;


namespace Room2Room.Models.UserNotification
{
    [Table("UserNotification")]
    public class UserNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string Message { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}