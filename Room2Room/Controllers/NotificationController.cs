using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Room2Room.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Room2Room.Services.Observers;

namespace Room2Room.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dismiss(int id)
        {
            var accountIdClaim = User.FindFirstValue("AccountId");

            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return RedirectToAction("LogIn", "Account");
            }

            int userId = int.Parse(accountIdClaim);

            var notification = await _context.UserNotifications.FindAsync(id);

            if (notification == null || notification.UserId != userId)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());

        }
    }
}