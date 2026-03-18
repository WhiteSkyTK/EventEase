using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login() => View();

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // DEBUG: See what the user typed
            Console.WriteLine($"--- Login Attempt: {email} ---");

            var user = await _context.Staff.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                // DEBUG: Check if the hash is truncated (should be ~84+ chars)
                Console.WriteLine($"User Found! Role: {user.Role}");
                Console.WriteLine($"Stored Hash Length: {user.Password.Length}");
                Console.WriteLine($"Stored Hash: {user.Password}");

                var hasher = new PasswordHasher<Staff>();
                var result = hasher.VerifyHashedPassword(user, user.Password, password);

                // DEBUG: See exactly why it failed
                Console.WriteLine($"Verification Result: {result}");

                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("UserRole", user.Role);
                    return RedirectToAction("Index", "Home");
                }
                else if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    Console.WriteLine("Warning: Password verified but needs rehash.");
                }
            }
            else
            {
                Console.WriteLine("User not found in database.");
            }

            ViewBag.Error = "Invalid Login Credentials! ❌";
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");

            // Security check: If not logged in, kick to login page
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login");
            }

            // ONLY Admins need these counts, so we only query the DB if they are an Admin
            if (role == "Admin")
            {
                ViewBag.VenueCount = await _context.Venues.CountAsync();
                ViewBag.BookingCount = await _context.Bookings.CountAsync();
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult StaffList()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login");

            var staff = _context.Staff.ToList();
            return View(staff);
        }

        // GET: Account/AddStaff
        public IActionResult AddStaff()
        {
            // Security Bouncer: Only Admins allowed
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Account/AddStaff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStaff(Staff staff)
        {
            if (ModelState.IsValid)
            {
                var exists = _context.Staff.Any(s => s.Email == staff.Email);
                if (exists)
                {
                    ViewBag.Error = "This email is already registered! 🛑";
                    return View(staff);
                }

                // --- THE FIX: HASH THE PASSWORD HERE ---
                var hasher = new PasswordHasher<Staff>();
                staff.Password = hasher.HashPassword(staff, staff.Password);

                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction("StaffList");
            }
            return View(staff);
        }
    }
}