using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Login(string email, string password)
        {
            var user = _context.Staff.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Store the user role in the Session
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserEmail", user.Email);

                return RedirectToAction("Dashboard", "Account");
            }

            ViewBag.Error = "Invalid Login Credentials! ❌";
            return View();
        }

        public IActionResult Dashboard()
        {
            // Security check: If someone tries to go to /Account/Dashboard without logging in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
            {
                return RedirectToAction("Login");
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
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                // Check if the email is already taken
                var exists = _context.Staff.Any(s => s.Email == staff.Email);
                if (exists)
                {
                    ViewBag.Error = "This email is already registered to a staff member! 🛑";
                    return View(staff);
                }

                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction("StaffList");
            }
            return View(staff);
        }
    }
}