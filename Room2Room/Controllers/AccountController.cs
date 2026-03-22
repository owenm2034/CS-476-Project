using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using Room2Room.Models.NotificationPreferences;

namespace Room2Room.Controllers;

public class AccountController : Controller
{
    private string ConnectionString;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public AccountController(
        ApplicationDbContext context,
        IEmailService emailService,
        IConfiguration configuration
    )
    {
        _context = context;
        _emailService = emailService;
        ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public IActionResult Index()
    {
        return NotFound();
    }

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

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            model.ErrorMessage = "Please enter your email.";
            model.Password = "";
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            model.ErrorMessage = "Please enter your password.";
            model.Password = "";
            return View(model);
        }

        var email = model.Email.Trim();

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Email == email);

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

        var restriction = await context.AccountRestrictions
            .FirstOrDefaultAsync(x => x.AccountId == account.Id);

        if (restriction != null)
        {
            if (restriction.Status == "Suspended")
            {
                model.ErrorMessage = "Your account has been suspended. Please contact an administrator.";
                model.Password = "";
                return View(model);
            }

            if (restriction.Status == "Deactivated")
            {
                model.ErrorMessage = "Your account has been deactivated. Please contact an administrator.";
                model.Password = "";
                return View(model);
            }
        }

        var university = await context.Universities
            .FirstOrDefaultAsync(u => u.Id == account.UniversityId);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.Username),
            new Claim("AccountId", account.Id.ToString()),
            new Claim("UniversityId", account.UniversityId.ToString()),
            new Claim("UniversityName", university?.Name ?? "")
        };

        if (account.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "User"));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return Redirect("/Listing/Index");
    }

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

        if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains("@"))
        {
            model.Password = "";
            model.ErrorMessage = "Please enter a valid university email.";
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Username))
        {
            model.Password = "";
            model.ErrorMessage = "Username is required.";
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            model.ErrorMessage = "Password is required.";
            return View(model);
        }

        var email = model.Email.Trim();
        var username = model.Username.Trim();
        var domain = email[(email.IndexOf("@") + 1)..];

        var university = await context.Universities
            .FirstOrDefaultAsync(x => x.Domain == domain);

        var emailExists = await context.Accounts
            .AnyAsync(a => a.Email == email);

        var usernameLower = username.ToLower();
        var usernameExists = await context.Accounts
            .AnyAsync(a => a.Username.ToLower() == usernameLower);

        bool isError = false;
        var errorMessage = "";
        string profilePicturePath = "";

        if (university == null)
        {
            isError = true;
            errorMessage += "Please use a university email. ";
        }

        if (emailExists)
        {
            isError = true;
            errorMessage += "An account with this email already exists. ";
        }

        if (usernameExists)
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

        if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
        {
            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName =
                Guid.NewGuid() + "_" + Path.GetFileName(model.ProfilePicture.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfilePicture.CopyToAsync(fileStream);
            }

            profilePicturePath = "/uploads/" + uniqueFileName;
        }

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
            email: email,
            hashString,
            saltString,
            username,
            profilePicturePath,
            university!.Id
        );

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        await _emailService.SendWelcomeEmailAsync(email, username);

        return Redirect("/Account/LogIn");
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Manage()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        var accountIdClaim = ((ClaimsIdentity)User.Identity!).FindFirst("AccountId")?.Value;
        if (!int.TryParse(accountIdClaim, out var accountId))
        {
            return NotFound();
        }

        var account = context.Accounts.FirstOrDefault(a => a.Id == accountId);
        if (account == null)
        {
            return NotFound();
        }

        NotificationPreference pref =
            context.NotificationPreferences.FirstOrDefault(x => x.AccountId == accountId)
            ?? new NotificationPreference(accountId);

        var model = new ManageModel
        {
            Email = account.Email,
            Username = account.Username,
            CurrentProfilePictureUrl = account.ProfilePictureUrl,
            NotificationPreferences = pref
        };

        return PartialView("_Manage", model);
    }

    [HttpPost]
    public async Task<IActionResult> Manage(ManageModel model)
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        if (!ModelState.IsValid)
        {
            model.NotificationPreferences ??= new NotificationPreference(0);
            return PartialView("_Manage", model);
        }

        var accountIdClaim = ((ClaimsIdentity)User.Identity!).FindFirst("AccountId")?.Value;
        if (!int.TryParse(accountIdClaim, out var accountId))
        {
            model.ErrorMessage = "Account not found.";
            model.NotificationPreferences ??= new NotificationPreference(0);
            return PartialView("_Manage", model);
        }

        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            model.ErrorMessage = "Account not found.";
            model.NotificationPreferences ??= new NotificationPreference(accountId);
            return PartialView("_Manage", model);
        }

        model.NotificationPreferences ??= new NotificationPreference(accountId);

        bool isError = false;
        var errorMessage = "";
        var updatedFields = new List<string>();

        if (!string.IsNullOrWhiteSpace(model.Email) &&
            !model.Email.Equals(account.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (!model.Email.Contains("@"))
            {
                isError = true;
                errorMessage += "Please use a university email. ";
            }
            else
            {
                var newEmail = model.Email.Trim();
                var domain = newEmail[(newEmail.IndexOf("@") + 1)..];

                var university = await context.Universities
                    .FirstOrDefaultAsync(x => x.Domain == domain);

                var emailExists = await context.Accounts.AnyAsync(a =>
                    a.Email == newEmail && a.Id != account.Id
                );

                if (university == null)
                {
                    isError = true;
                    errorMessage += "Please use a university email. ";
                }

                if (emailExists)
                {
                    isError = true;
                    errorMessage += "An account with this email already exists. ";
                }

                if (!isError)
                {
                    account.Email = newEmail;
                    account.UniversityId = university!.Id;
                    updatedFields.Add("email address");
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Username) &&
            !model.Username.Equals(account.Username, StringComparison.OrdinalIgnoreCase))
        {
            var newUsername = model.Username.Trim();
            var newUsernameLower = newUsername.ToLower();

            var usernameExists = await context.Accounts.AnyAsync(a =>
                a.Username.ToLower() == newUsernameLower && a.Id != account.Id
            );

            if (usernameExists)
            {
                isError = true;
                errorMessage += "An account with this username already exists. ";
            }
            else
            {
                account.Username = newUsername;
                updatedFields.Add("username");
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            if (string.IsNullOrWhiteSpace(model.OldPassword))
            {
                isError = true;
                errorMessage += "You must provide your current password to set a new one. ";
            }
            else
            {
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
        }

        if (isError)
        {
            model.Password = "";
            model.OldPassword = "";
            model.ErrorMessage = errorMessage;
            return PartialView("_Manage", model);
        }

        try
        {
            var notif = await context.NotificationPreferences
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            if (notif == null)
            {
                notif = new NotificationPreference(accountId);
                context.NotificationPreferences.Add(notif);
            }

            bool notifChanged =
                notif.RecieveEmailNotificationOnChatMessageRecieved != model.NotificationPreferences.RecieveEmailNotificationOnChatMessageRecieved ||
                notif.RecieveEmailNotificationOnListingReported != model.NotificationPreferences.RecieveEmailNotificationOnListingReported ||
                notif.RecieveEmailNotificationOnUserReported != model.NotificationPreferences.RecieveEmailNotificationOnUserReported;

            notif.RecieveEmailNotificationOnChatMessageRecieved =
                model.NotificationPreferences.RecieveEmailNotificationOnChatMessageRecieved;

            if (User.IsInRole("Admin"))
            {
                notif.RecieveEmailNotificationOnListingReported =
                    model.NotificationPreferences.RecieveEmailNotificationOnListingReported;
                notif.RecieveEmailNotificationOnUserReported =
                    model.NotificationPreferences.RecieveEmailNotificationOnUserReported;
            }
            else
            {
                notif.RecieveEmailNotificationOnListingReported = false;
                notif.RecieveEmailNotificationOnUserReported = false;
            }

            if (notifChanged)
            {
                updatedFields.Add("notification preferences");
            }
        }
        catch (Exception ex)
        {
            model.Password = "";
            model.OldPassword = "";
            model.ErrorMessage = $"Error updating notification preferences: {ex.Message}";
            return PartialView("_Manage", model);
        }

        if (model.NewProfilePicture != null && model.NewProfilePicture.Length > 0)
        {
            try 
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.NewProfilePicture.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewProfilePicture.CopyToAsync(fileStream);
                }

                // Erase earlier profile picture file if it exists
                if (!string.IsNullOrEmpty(account.ProfilePictureUrl))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", account.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // Update account with new profile picture path
                account.ProfilePictureUrl = "/uploads/" + uniqueFileName;
                model.CurrentProfilePictureUrl = account.ProfilePictureUrl;
                updatedFields.Add("profile picture");
            }
            catch (Exception ex)
            {
                isError = true;
                errorMessage += $"Error saving profile picture: {ex.Message} ";
            }
        }
        if (isError) 
        {
            model.ErrorMessage = errorMessage;
            model.Password = "";
            model.OldPassword = "";
            return PartialView("_Manage", model); 
        }


        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            model.Password = "";
            model.OldPassword = "";
            model.ErrorMessage = $"Error saving changes: {ex.Message}";
            return PartialView("_Manage", model);
        }

        string successMessage;
        if (updatedFields.Count == 0)
        {
            successMessage = "No changes made.";
        }
        else if (updatedFields.Count == 1)
        {
            successMessage = "The field " + updatedFields[0] + " was successfully updated.";
        }
        else
        {
            successMessage =
                "The fields " + string.Join(", ", updatedFields) + " were successfully updated.";
        }

        model.Password = "";
        model.OldPassword = "";
        model.SuccessMessage = successMessage;
        return PartialView("_Manage", model);
    }
}