using Microsoft.AspNetCore.Mvc;
using EventEase.Data;
using Microsoft.AspNetCore.Http;

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
    }
}