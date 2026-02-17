using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models.Accounts;
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
    public IActionResult LogIn(LogInModel model)
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new ApplicationDbContext(contextOptions);

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
            model.Email,
            model.Username,
            hashString,
            saltString,
            "" // todo, save profile picture
        );

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Redirect("/Account/LogIn");
    }

    private static string Hash(string input)
    {
        using (var sha = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
