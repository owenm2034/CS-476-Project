using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Room2Room;
using Room2Room.Data;
using Resend;
using Room2Room.Models;
using Room2Room.Services;
using Room2Room.Services.Observers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString)
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var mvcBuilder = builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Cookie.Name = "MyAuthCookie";
    });

builder.Services.AddTransient<IListingRepository, ListingRepository>();
builder.Services.AddScoped<IWatchlistRepository, WatchlistRepository>();

// Observer Pattern Services
builder.Services.AddScoped<IItemObserver, DbPriceDropObserver>();
builder.Services.AddScoped<IItemObserver, DbStatusChangeObserver>();
builder.Services.AddScoped<IItemObserver, EmailPriceDropObserver>();
builder.Services.AddScoped<IItemObserver, EmailStatusChangeObserver>();
builder.Services.AddScoped<IItemSubject, ItemSubject>();

// Email Service- ReSend
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
