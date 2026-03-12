using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Room2Room.Models.Chats;

public class ChatMember
{
    [Key]
    public int ChatMemberId { get; set; }
    public int ChatId { get; set; }
    public int AccountId { get; set; }
}