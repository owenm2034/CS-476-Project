namespace Room2Room.Models.Accounts;

public class Administrator : User
{
    public override bool IsAdmin { get; set; } = true;
}
