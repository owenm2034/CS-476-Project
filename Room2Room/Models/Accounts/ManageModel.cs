namespace Room2Room.Models.Accounts;

public class ManageModel
{
    // copied from LogInModel.cs for the time being. refine later
    public string Email { get; set; }
    public string Password { get; set; }
    public string ErrorMessage { get; set; } = "";
}