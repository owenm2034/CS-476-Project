namespace Room2Room.Models.Accounts;

public class User : Account
{
    public override bool IsAdmin { get; set; } = false;
}
