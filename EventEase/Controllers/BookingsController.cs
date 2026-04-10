using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsStaff()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin" || role == "Specialist";
        }

        public async Task<IActionResult> Index(string searchString, int? eventTypeId, DateTime? startDate, DateTime? endDate, bool onlyAvailable = false)
        {
            // 1. Include everything so we can see the Event Type and Venue status
            var query = _context.Bookings
                .Include(b => b.Event).ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            // 2. SEARCH: Handle Customer Name or "#EE-1" Booking ID
            if (!string.IsNullOrEmpty(searchString))
            {
                // Clean the search string: Remove "#EE-" or "EE-" and any extra spaces
                string cleanString = searchString.ToUpper().Replace("#EE-", "").Replace("EE-", "").Trim();

                // Now check if what's left is a number
                bool isId = int.TryParse(cleanString, out int searchId);

                query = query.Where(b => b.CustomerName.Contains(searchString) ||
                                         b.CustomerSurname.Contains(searchString) ||
                                         (isId && b.BookingId == searchId));
            }

            // 3. FILTER: Event Type (The lookup we added)
            if (eventTypeId.HasValue)
            {
                query = query.Where(b => b.Event.EventTypeId == eventTypeId.Value);
            }

            // 4. FILTER: Date Range
            if (startDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= endDate.Value);
            }

            // 5. FILTER: Venue Availability (Part 3 Requirement)
            if (onlyAvailable)
            {
                query = query.Where(b => b.Venue.IsAvailable == true);
            }

            // Prepare data for the dropdown and UI
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeId", "TypeName", eventTypeId);
            ViewData["CurrentFilter"] = searchString;

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!IsStaff()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var booking = await _context.Bookings.Include(b => b.Event).Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            return booking == null ? NotFound() : View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            if (!IsStaff()) return RedirectToAction("Index", "Home");
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");

            // NEW: Fetch upcoming bookings so the Specialist can see them
            ViewBag.UpcomingBookings = _context.Bookings
                .Include(b => b.Venue)
                .Where(b => b.BookingDate >= DateTime.Today)
                .OrderBy(b => b.BookingDate)
                .ToList();

            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            // Fix: Ignore navigation properties for validation
            ModelState.Remove("Event");
            ModelState.Remove("Venue");

            // 1. Availability Check
            var isTaken = await _context.Bookings.AnyAsync(b =>
                b.VenueId == booking.VenueId && b.BookingDate.Date == booking.BookingDate.Date);

            if (isTaken)
            {
                ModelState.AddModelError("BookingDate", "🛑 Venue already booked for this date!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // --- THE BUG FIX: RELOAD DROPDOWNS HERE ---
            // If we reach this point, it means something failed. We must reload the lists 
            // or the dropdowns will be empty when the page reloads!
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Location", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate,CustomerName,CustomerSurname,CustomerPhone")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            // REMOVE validation for Event and Venue objects so the update can proceed
            ModelState.Remove("Event");
            ModelState.Remove("Venue");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(e => e.BookingId == booking.BookingId)) return NotFound();
                    else throw;
                }
            }

            // If we got here, something failed; reload the dropdowns
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Location", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.Include(b => b.Event).FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking != null)
            {
                // 48-Hour Cancellation Rule
                var hoursUntilEvent = (booking.Event.StartDateTime - DateTime.Now).TotalHours;

                if (hoursUntilEvent < 48)
                {
                    TempData["Error"] = "Cancellations must be made at least 48 hours in advance! 🛑";
                    return RedirectToAction(nameof(Index));
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
