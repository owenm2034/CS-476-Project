using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models;

namespace Room2Room.Controllers
{
    public class ListingsController : Controller   // responsible for all user actions (show all listings, create new listing, save new listing)
    {
         private readonly ApplicationDbContext _context;

        public ListingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // handle get request
        public async Task<IActionResult> Index()
        {
            var listings = await _context.Listings.ToListAsync();
            return View(listings);
        }

        // handle create request, return the form to create a request
        public IActionResult Create()
        {
            return View();
        }

        // save data from create request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Listing listing)
        {
            if (ModelState.IsValid) //form validation 
            {
                _context.Add(listing); //add to the listing table
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); //return to the index page to see all listings
            }
            return View(listing);
        }
    }
    
}