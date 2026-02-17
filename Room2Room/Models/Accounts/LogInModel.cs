namespace Room2Room.Models.Accounts;

public class LogInModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ErrorMessage { get; set; } = "";
}
