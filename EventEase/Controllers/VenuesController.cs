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

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Include the Bookings so we can see which dates are taken
            var venue = await _context.Venues
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null) return NotFound();

            return View(venue);
        }

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
            // 1. Clear navigation properties so they don't cause false validation errors
            ModelState.Remove("Events");
            ModelState.Remove("Bookings");

            // 2. Image Validation Logic
            if (imageFile != null && imageFile.Length > 0)
            {
                var maxFileSize = 5 * 1024 * 1024; // 5 MB
                if (imageFile.Length > maxFileSize)
                    ModelState.AddModelError("ImageUrl", "❌ The image is too large. Maximum size is 5MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    ModelState.AddModelError("ImageUrl", "❌ Only JPG, PNG, and WEBP images are allowed.");

                // If it passed validation, upload it!
                if (ModelState.IsValid)
                {
                    string uploadedUrl = await _blobService.UploadImageAsync(imageFile, "venue-images");
                    venue.ImageUrl = uploadedUrl;
                }
            }

            // 3. Save if everything is perfectly valid
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

            ModelState.Remove("Events");
            ModelState.Remove("Bookings");

            if (imageFile != null && imageFile.Length > 0)
            {
                var maxFileSize = 5 * 1024 * 1024; // 5 MB
                if (imageFile.Length > maxFileSize)
                    ModelState.AddModelError("ImageUrl", "❌ The image is too large. Maximum size is 5MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    ModelState.AddModelError("ImageUrl", "❌ Only JPG, PNG, and WEBP images are allowed.");

                if (ModelState.IsValid)
                {
                    string uploadedUrl = await _blobService.UploadImageAsync(imageFile, "venue-images");
                    venue.ImageUrl = uploadedUrl;
                }
            }

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