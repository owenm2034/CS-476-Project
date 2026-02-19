using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models.Accounts;

public class RegisterModel
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
    public IFormFile? ProfilePicture { get; set; }
    public string? ErrorMessage { get; set; }
}
