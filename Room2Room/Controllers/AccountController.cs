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
    private const string ConnectionString =
        @"Server=localhost,1433;Database=Room2Room;User Id=sa;Password=aStrong!Passw0rd;TrustServerCertificate=True;";
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return NotFound();
    }

    [HttpGet]
    public IActionResult LogIn()
    {
        return View("LogIn");
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

        var university = context.Universities.Where(u => u.Id == account.UniversityId).FirstOrDefault();

        // set claims
        var claims = new List<Claim> { 
            new Claim(ClaimTypes.Name, account.Username), 
            new Claim("UniversityId", account.UniversityId.ToString()),
            new Claim("UniversityName", university?.Name ?? ""),
        };

        if (account.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        } else {
            claims.Add(new Claim(ClaimTypes.Role, "User"));
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
        return View("Register");
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

        var domain = model.Email.Substring(model.Email.IndexOf("@") + 1);

        var university = context.Universities.Where(x => x.Domain == domain).FirstOrDefault();

        if (university == null)
        {
            model.Password = "";
            model.Email = "";
            model.ErrorMessage = "Please use a university email.";
            return View(model);
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
            email: model.Email,
            hashString,
            saltString,
            model.Username,
            "", // todo, save profile picture,
            university.Id
        );

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Redirect("/Account/LogIn");
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("LogIn", "Account");
    }
}
