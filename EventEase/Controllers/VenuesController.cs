using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Http; // Required for Session
using EventEase.Services; // Add this

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;

        public VenuesController(ApplicationDbContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        // GET: Venues
        public async Task<IActionResult> Index(string searchString, int? minCapacity)
        {
            // 1. Start with all venues
            var venues = from v in _context.Venues
                         select v;

            // 2. Filter by Name if a search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                venues = venues.Where(s => s.VenueName.Contains(searchString) || s.Location.Contains(searchString));
            }

            // 3. Filter by Capacity if a number is provided
            if (minCapacity.HasValue)
            {
                venues = venues.Where(v => v.Capacity >= minCapacity.Value);
            }

            // Pass the search values back to the view so the text boxes don't clear out
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCapacity"] = minCapacity;

            return View(await venues.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id) => id == null ? NotFound() : View(await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id));

        // GET: Venues/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            // Fix: Send an empty Venue object to the view
            return View(new Venue());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile? imageFile)
        {
            // 1. Check if a physical file was uploaded
            if (imageFile != null && imageFile.Length > 0)
            {
                // Upload to Azure and get the new URL
                string uploadedUrl = await _blobService.UploadImageAsync(imageFile, "venue-images");
                venue.ImageUrl = uploadedUrl;
            }

            // 2. If no file was uploaded, it will just use whatever is in venue.ImageUrl (the text box)

            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var venue = await _context.Venues.FindAsync(id);
            return venue == null ? NotFound() : View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile? imageFile)
        {
            if (id != venue.VenueId) return NotFound();
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            // 1. Logic: If a new file is uploaded, use it. 
            // If not, it keeps whatever is in the ImageUrl text box (which could be the old URL).
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadedUrl = await _blobService.UploadImageAsync(imageFile, "venue-images");
                venue.ImageUrl = uploadedUrl;
            }

            // 2. Clear validation for navigation properties
            ModelState.Remove("Events");
            ModelState.Remove("Bookings");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venues.Any(e => e.VenueId == venue.VenueId)) return NotFound();
                    else throw;
                }
            }
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            return venue == null ? NotFound() : View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            // CEO REQUIREMENT: Restrict Deletion if Bookings exist
            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["Error"] = "❌ CANNOT DELETE: This venue is linked to active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue != null) _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}