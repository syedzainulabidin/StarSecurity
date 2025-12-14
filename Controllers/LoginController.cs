using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using StarSecurity.Data;
using StarSecurity.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace StarSecurity.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /login
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /login
        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == email);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.Password))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role),
                new Claim("EmployeeId", employee.Id.ToString()),
                new Claim("Name", employee.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect to dashboard
            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /accessdenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}