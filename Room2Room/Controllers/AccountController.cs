using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Room2Room.Controllers;

public class AccountController : Controller
{
    // TODO: move out of controller, move db connection instantiation into factory
    private string ConnectionString;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public AccountController(ApplicationDbContext context, IEmailService emailService, IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;

        ConnectionString = configuration.GetConnectionString("DefaultConnection");
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

        return Redirect("/Home/Privacy");
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

        //Send Welcome Email
        await _emailService.SendWelcomeEmailAsync(model.Email, model.Username);

        return Redirect("/Account/LogIn");
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("LogIn", "Account");
    }
}
