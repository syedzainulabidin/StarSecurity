using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Authorize(Roles = "admin")]
    public class ApplicationController : Controller
    {
        private readonly AppDbContext _context;

        public ApplicationController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var applications = _context.Hirings
                .Include(h => h.Qualification)
                .Include(h => h.Vacancy)
                    .ThenInclude(v => v.Service)
                .OrderByDescending(h => h.CreatedAt)
                .ToList();

            return View(applications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Hire(int id)
        {
            var application = _context.Hirings
                .Include(h => h.Vacancy)
                .FirstOrDefault(h => h.Id == id);

            if (application == null) return NotFound();


            application.Status = "Hired";
            application.UpdatedAt = DateTime.Now;

            var employee = new Employee
            {
                Name = application.Name,
                Email = application.Email,
                Password = BCrypt.Net.BCrypt.HashPassword("Default@123"),
                Contact = application.Contact,
                Address = application.Address,
                QualificationId = application.QualificationId,
                ServiceId = application.Vacancy.ServiceId,
                Grade = "Trainee",
                Role = "staff",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();

            TempData["Message"] = "Application marked as Hired and employee created.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            var application = _context.Hirings.Find(id);
            if (application == null) return NotFound();

            application.Status = "Rejected";
            application.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Message"] = "Application rejected.";
            return RedirectToAction("Index");
        }
    }
}