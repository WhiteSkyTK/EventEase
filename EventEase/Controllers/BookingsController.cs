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

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var bookings = _context.Bookings.Include(b => b.Event).Include(b => b.Venue).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b => b.Event.EventName.Contains(searchString)
                                            || b.BookingId.ToString().Contains(searchString));
            }

            return View(await bookings.ToListAsync());
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
        // Added the new customer fields to the [Bind] list
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate,CustomerName,CustomerSurname,CustomerPhone")] Booking booking)
        {
            ModelState.Remove("Event");
            ModelState.Remove("Venue");

            // 1. Double Booking Check
            var alreadyBooked = await _context.Bookings
                .AnyAsync(b => b.VenueId == booking.VenueId && b.BookingDate.Date == booking.BookingDate.Date);

            if (alreadyBooked)
            {
                ModelState.AddModelError("BookingDate", "🛑 This venue is already occupied on this date!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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
