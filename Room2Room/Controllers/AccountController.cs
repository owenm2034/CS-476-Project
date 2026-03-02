using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Room2Room.Models.NotificationPreferences;

namespace Room2Room.Controllers;

public class AccountController : Controller
{
    // TODO: move out of controller, move db connection instantiation into factory
    private string ConnectionString;
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IActionResult Index()
    {
        return NotFound();
    }

    // Log In GET/POST functions
    [HttpGet]
    public IActionResult LogIn()
    {
        var model = new LogInModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LogIn(LogInModel model)
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        // get account
        var account = context.Accounts.Where(a => a.Email == model.Email).FirstOrDefault();

        if (account == null)
        {
            model.ErrorMessage = "Invalid credentials. Please try again.";
            model.Password = "";
            return View(model);
        }

        byte[] salt = Convert.FromBase64String(account.PasswordSalt);
        byte[] expectedHash = Convert.FromBase64String(account.PasswordHash);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            model.Password,
            salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: expectedHash.Length
        );

        if (!CryptographicOperations.FixedTimeEquals(actualHash, expectedHash))
        {
            model.ErrorMessage = "Invalid credentials. Please try again.";
            model.Password = "";
            return View(model);
        }

        var university = context.Universities
            .Where(u => u.Id == account.UniversityId)
            .FirstOrDefault();

        // set claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.Username), //
            new Claim("AccountId", account.Id.ToString()), // use ((ClaimsIdentity)User.Identity).FindFirst("AccountId").Value
            new Claim("UniversityId", account.UniversityId.ToString()), //  use ((ClaimsIdentity)User.Identity).FindFirst("UniversityId").Value
            new Claim("UniversityName", university?.Name ?? ""), // use ((ClaimsIdentity)User.Identity).FindFirst("UniversityName").Value
        };

        if (account.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin")); // use User.IsInRole("Admin")
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "User")); // use User.IsInRole("User")
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect("/Listing/Index");
    }

    // Register GET/POST functions
    [HttpGet]
    public IActionResult Register()
    {
        var model = new RegisterModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var domain = model.Email.Substring(model.Email.IndexOf("@") + 1);

        var universityExistsTask = context.Universities.Where(x => x.Domain == domain).ToList();
        var emailExistsTask = context.Accounts.Where(a => a.Email == model.Email).ToList();
        var usernameExistsTask = context.Accounts
            .Where(a => a.Username.ToLower() == model.Username.ToLower())
            .ToList();

        bool isError = false;
        var errorMessage = "";

        if (universityExistsTask.Count == 0)
        {
            isError = true;
            errorMessage += "Please use a university email. ";
        }
        if (emailExistsTask.Count > 0)
        {
            isError = true;
            errorMessage += "An account with this email already exists. ";
        }
        if (usernameExistsTask.Count > 0)
        {
            isError = true;
            errorMessage += "An account with this username already exists. ";
        }
        if (model.Password.Length < 8)
        {
            isError = true;
            errorMessage += "Your password must be at least 8 characters. ";
        }

        if (isError)
        {
            model.Password = "";
            model.Email = "";
            model.ErrorMessage = errorMessage;
            return View(model);
        }

        // Validation passed, create account here
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            model.Password,
            salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        string saltString = Convert.ToBase64String(salt);
        string hashString = Convert.ToBase64String(hash);

        var account = new Account(
            email: model.Email,
            hashString,
            saltString,
            model.Username,
            "", // todo, save profile picture,
            universityExistsTask.First().Id
        );

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Redirect("/Account/LogIn");
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // Manage Account GET/POST functions
    [HttpGet]
    public IActionResult Manage() // GET
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        // get current user's account using AccountId claim
        var accountIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("AccountId")?.Value;
        if (!int.TryParse(accountIdClaim, out var accountId))
        {
            return NotFound();
        }

        var account = context.Accounts.Where(a => a.Id == accountId).FirstOrDefault();
        if (account == null)
        {
            return NotFound();
        }

        NotificationPreference pref =
            context.NotificationPreferences.Where(x => x.AccountId == accountId).FirstOrDefault()
            ?? new NotificationPreference(accountId);

        // pre-populate model with current user data
        var model = new ManageModel
        {
            Email = account.Email,
            Username = account.Username,
            NotificationPreferences = pref
        };

        return PartialView("_Manage", model); // renders Manage.cshtml
    }

    [HttpPost]
    public async Task<IActionResult> Manage(ManageModel model) // POST
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        if (!ModelState.IsValid)
        {
            return PartialView("_Manage", model);
        }

        // get account using AccountId claim
        var accountIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("AccountId")?.Value;
        if (!int.TryParse(accountIdClaim, out var accountId))
        {
            model.ErrorMessage = "Account not found.";
            return PartialView("_Manage", model);
        }

        var account = context.Accounts.Where(a => a.Id == accountId).FirstOrDefault();
        if (account == null)
        {
            model.ErrorMessage = "Account not found.";
            return PartialView("_Manage", model);
        }

        bool isError = false;
        var errorMessage = "";

        // track what fields were changed so we can craft a success message later
        var updatedFields = new List<string>();

        // Email validation
        if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != account.Email)
        {
            var domain = model.Email.Substring(model.Email.IndexOf("@") + 1);

            var universityExistsTask = context.Universities.Where(x => x.Domain == domain).ToList();
            var emailExistsTask = context.Accounts
                .Where(a => a.Email == model.Email && a.Id != account.Id)
                .ToList();

            if (universityExistsTask.Count == 0)
            {
                isError = true;
                errorMessage += "Please use a university email. ";
            }
            if (emailExistsTask.Count > 0)
            {
                isError = true;
                errorMessage += "An account with this email already exists. ";
            }

            // If no error, update account email
            if (!isError)
            {
                account.Email = model.Email;
                updatedFields.Add("email address");
            }
        }

        // Username validation
        if (!string.IsNullOrWhiteSpace(model.Username) && model.Username != account.Username)
        {
            var usernameExistsTask = context.Accounts
                .Where(a => a.Username.ToLower() == model.Username.ToLower() && a.Id != account.Id)
                .ToList();

            if (usernameExistsTask.Count > 0)
            {
                isError = true;
                errorMessage += "An account with this username already exists. ";
            }
            else
            {
                account.Username = model.Username;
                updatedFields.Add("username");
            }
        }

        // Password validation
        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            if (string.IsNullOrWhiteSpace(model.OldPassword))
            {
                isError = true;
                errorMessage += "You must provide your current password to set a new one. ";
            }
            else
            {
                // verify that OldPassword matches the existing hash
                byte[] salt = Convert.FromBase64String(account.PasswordSalt);
                byte[] expectedHash = Convert.FromBase64String(account.PasswordHash);
                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    model.OldPassword,
                    salt,
                    iterations: 100_000,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    outputLength: expectedHash.Length
                );

                if (!CryptographicOperations.FixedTimeEquals(actualHash, expectedHash))
                {
                    isError = true;
                    errorMessage += "Current password is incorrect. ";
                }
                else if (model.Password.Length < 8)
                {
                    isError = true;
                    errorMessage += "Your password must be at least 8 characters. ";
                }
                else
                {
                    // Hash and update password
                    byte[] newSalt = RandomNumberGenerator.GetBytes(16);
                    byte[] newHash = Rfc2898DeriveBytes.Pbkdf2(
                        model.Password,
                        newSalt,
                        iterations: 100_000,
                        hashAlgorithm: HashAlgorithmName.SHA256,
                        outputLength: 32
                    );

                    account.PasswordSalt = Convert.ToBase64String(newSalt);
                    account.PasswordHash = Convert.ToBase64String(newHash);
                    updatedFields.Add("password");
                }
            }

            if (isError)
            {
                model.Password = "";
                model.OldPassword = "";
                // model.Email = "";
                model.ErrorMessage = errorMessage;
                return PartialView("_Manage", model);
            }
        }

        // notification preferences
        try
        {
            var notifUpdated = false;
            var notif = context.NotificationPreferences
                .Where(x => x.AccountId == accountId)
                .FirstOrDefault();
            if (notif == null)
            {
                notifUpdated = true;
                notif = new NotificationPreference(accountId);
                context.NotificationPreferences.Add(notif);
            }

            if (notif.RecieveEmailNotificationOnChatMessageRecieved != model.NotificationPreferences.RecieveEmailNotificationOnChatMessageRecieved
            || notif.RecieveEmailNotificationOnListingReported != model.NotificationPreferences.RecieveEmailNotificationOnListingReported
            || notif.RecieveEmailNotificationOnUserReported != model.NotificationPreferences.RecieveEmailNotificationOnUserReported)
            {
                notifUpdated = true;
                updatedFields.Add("notification preferences");
            }

            notif.RecieveEmailNotificationOnChatMessageRecieved = model
                .NotificationPreferences
                .RecieveEmailNotificationOnChatMessageRecieved;
            if (User.IsInRole("Admin"))
            {
                notif.RecieveEmailNotificationOnListingReported = model
                    .NotificationPreferences
                    .RecieveEmailNotificationOnListingReported;
                notif.RecieveEmailNotificationOnUserReported = model
                    .NotificationPreferences
                    .RecieveEmailNotificationOnUserReported;
            }
            else
            {
                notif.RecieveEmailNotificationOnListingReported = false;
                notif.RecieveEmailNotificationOnUserReported = false;
            }
        }
        catch (Exception ex) { }

        // Save changes to database
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            model.Password = "";
            model.ErrorMessage = $"Error saving changes: {ex.Message}";
            return PartialView("_Manage", model);
        }

        // build a success string based on which fields changed
        string successMessage;
        if (updatedFields.Count == 0) // no fields changed
        {
            successMessage = "No changes made.";
        }
        else if (updatedFields.Count == 1) // 1 field changed
        {
            successMessage = "The field " + updatedFields[0] + " was successfully updated.";
        }
        else // > 1 field changed
        {
            successMessage =
                "The fields " + string.Join(", ", updatedFields) + " were successfully updated.";
        }

        model.Password = "";
        model.SuccessMessage = successMessage;
        return PartialView("_Manage", model); // redirects to GET Manage after success
    }
}
