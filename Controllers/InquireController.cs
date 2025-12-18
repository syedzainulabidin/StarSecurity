using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;
using BCrypt.Net;

namespace StarSecurity.Controllers
{
    public class InquireController : Controller
    {
        private readonly AppDbContext _context;

        public InquireController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /inquire
        public IActionResult Index()
        {
            return View();
        }

        // POST: /inquire/check
        [HttpPost]
        public IActionResult Check(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Please enter a valid email.";
                return View("Index");
            }

            // 1. Check in Bookings
            var bookings = _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Employee)
                .Where(b => b.ClientEmail == email && b.Status != "Completed")
                .ToList();

            if (bookings.Any())
            {
                ViewBag.Bookings = bookings;
                ViewBag.Type = "booking";
                return View("Index");
            }

            // 2. Check in Job Applications (Hirings)
            var applications = _context.Hirings
                .Include(h => h.Vacancy)
                    .ThenInclude(v => v.Service)
                .Where(h => h.Email == email)
                .ToList();

            if (applications.Any())
            {
                // Prepare list of application info with default password flag
                var appResults = new List<object>();
                foreach (var app in applications)
                {
                    bool isDefaultPassword = false;
                    if (app.Status == "Hired")
                    {
                        var employee = _context.Employees
                            .FirstOrDefault(e => e.Email == app.Email);
                        if (employee != null)
                        {
                            isDefaultPassword = BCrypt.Net.BCrypt.Verify("Default@123", employee.Password);
                        }
                    }

                    appResults.Add(new
                    {
                        App = app,
                        IsDefaultPassword = isDefaultPassword
                    });
                }

                ViewBag.AppResults = appResults;
                ViewBag.Type = "application";
                return View("Index");
            }

            // No records found
            ViewBag.NotFound = true;
            return View("Index");
        }
    }
}