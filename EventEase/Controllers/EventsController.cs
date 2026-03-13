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

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

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
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View(new Event()); // Fix: Pass empty object to prevent NullRef in view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,Description,StartDateTime,EndDateTime,VenueId,ImageUrl")] Event eventItem, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            // 1. Handle Image Upload
            if (imageFile != null && imageFile.Length > 0)
            {
                eventItem.ImageUrl = await _blobService.UploadImageAsync(imageFile, "event-images");
            }

            // 2. Prevent Double Bookings
            bool isDoubleBooked = await _context.Events.AnyAsync(e =>
                e.VenueId == eventItem.VenueId &&
                eventItem.StartDateTime < e.EndDateTime &&
                e.StartDateTime < eventItem.EndDateTime);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("", "⚠️ This venue is already booked for the selected time slot!");
            }

            ModelState.Remove("Venue");

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
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var eventItem = await _context.Events.FindAsync(id);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", eventItem?.VenueId);
            return eventItem == null ? NotFound() : View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Description,StartDateTime,EndDateTime,VenueId,ImageUrl")] Event eventItem, IFormFile? imageFile)
        {
            if (id != eventItem.EventId || !IsAdmin()) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new image is uploaded, use it. Otherwise, keep the old ImageUrl
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        eventItem.ImageUrl = await _blobService.UploadImageAsync(imageFile, "event-images");
                    }

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
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            return View(await _context.Events.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventId == id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var item = await _context.Events.FindAsync(id);
            if (item != null) _context.Events.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // FIX: Removed the role check so customers can view this page!
            if (id == null) return NotFound();

            var eventItem = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }
    }
}