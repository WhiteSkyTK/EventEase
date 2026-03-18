using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;

        public EventsController(ApplicationDbContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        private bool IsAuthorized()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin" || role == "Specialist";
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var events = from e in _context.Events.Include(e => e.Venue) select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(s => s.EventName.Contains(searchString));
            }

            return View(await events.ToListAsync());
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View(new Event()); // Fix: Pass empty object to prevent NullRef in view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,Description,StartDateTime,EndDateTime,VenueId,ImageUrl")] Event eventItem, IFormFile? imageFile)
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // 1. Clear navigation properties so they don't break validation
            ModelState.Remove("Venue");
            ModelState.Remove("Bookings");
            ModelState.Remove("ImageUrl"); // Remove this so file uploads don't trigger string required errors

            // 2. Double Booking Check (Time Slot)
            if (eventItem.VenueId.HasValue)
            {
                bool isDoubleBooked = await _context.Events.AnyAsync(e =>
                    e.VenueId == eventItem.VenueId &&
                    eventItem.StartDateTime < e.EndDateTime &&
                    e.StartDateTime < eventItem.EndDateTime);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("StartDateTime", "⚠️ This venue is already booked for this time slot!");
                }
            }

            // 3. Image Validation & Upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var maxFileSize = 5 * 1024 * 1024; // 5 MB
                if (imageFile.Length > maxFileSize)
                    ModelState.AddModelError("ImageUrl", "❌ The image is too large. Maximum size is 5MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    ModelState.AddModelError("ImageUrl", "❌ Only JPG, PNG, and WEBP images are allowed.");

                // If no errors so far, upload to Azure!
                if (ModelState.IsValid)
                {
                    eventItem.ImageUrl = await _blobService.UploadImageAsync(imageFile, "event-images");
                }
            }

            // 4. Final Save
            if (ModelState.IsValid)
            {
                _context.Add(eventItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            return View(eventItem);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            var eventItem = await _context.Events.FindAsync(id);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem?.VenueId);
            return eventItem == null ? NotFound() : View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Description,StartDateTime,EndDateTime,VenueId,ImageUrl")] Event eventItem, IFormFile? imageFile)
        {
            if (id != eventItem.EventId || !IsAuthorized()) return NotFound();

            ModelState.Remove("Venue");
            ModelState.Remove("Bookings");
            ModelState.Remove("ImageUrl");

            // Double Booking Check (Ignore the current event being edited)
            if (eventItem.VenueId.HasValue)
            {
                bool isDoubleBooked = await _context.Events.AnyAsync(e =>
                    e.VenueId == eventItem.VenueId &&
                    e.EventId != eventItem.EventId &&
                    eventItem.StartDateTime < e.EndDateTime &&
                    e.StartDateTime < eventItem.EndDateTime);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("StartDateTime", "⚠️ This venue is already booked for this time slot!");
                }
            }

            // Image Validation & Upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var maxFileSize = 5 * 1024 * 1024;
                if (imageFile.Length > maxFileSize)
                    ModelState.AddModelError("ImageUrl", "❌ The image is too large. Maximum size is 5MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    ModelState.AddModelError("ImageUrl", "❌ Only JPG, PNG, and WEBP images are allowed.");

                if (ModelState.IsValid)
                {
                    eventItem.ImageUrl = await _blobService.UploadImageAsync(imageFile, "event-images");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.EventId == eventItem.EventId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem.VenueId);
            return View(eventItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");
            return View(await _context.Events.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventId == id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            // --- NEW REQUIREMENT CODE: Prevent deletion if bookings exist ---
            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
            if (hasBookings)
            {
                TempData["Error"] = "❌ CANNOT DELETE: This event currently has active ticket bookings.";
                return RedirectToAction(nameof(Index));
            }
            // -----------------------------------------------------------------

            var item = await _context.Events.FindAsync(id);
            if (item != null) _context.Events.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }
    }
}