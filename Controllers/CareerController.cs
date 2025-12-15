using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    public class CareerController : Controller
    {
        private readonly AppDbContext _context;

        public CareerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /careers/apply?serviceId=5
        [HttpGet]
        public IActionResult Apply(int serviceId)
        {
            var service = _context.Services.Find(serviceId);
            if (service == null) return NotFound();

            // Find the first OPEN vacancy for this service
            var vacancy = _context.Vacancies
                .FirstOrDefault(v => v.ServiceId == serviceId && v.Status == "Open");

            if (vacancy == null)
            {
                TempData["Error"] = "No open vacancy for this service.";
                return RedirectToAction("Careers", "Home");
            }

            ViewBag.ServiceTitle = service.Title;
            ViewBag.VacancyId = vacancy.Id;
            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            return View();
        }

        // POST: /careers/apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(Hiring hiring)
        {
            ModelState.Remove("Qualification");
            ModelState.Remove("Vacancy");

            if (ModelState.IsValid)
            {
                hiring.Status = "Pending";
                hiring.CreatedAt = DateTime.Now;
                hiring.UpdatedAt = DateTime.Now;

                _context.Hirings.Add(hiring);
                _context.SaveChanges();

                TempData["Success"] = "Application submitted successfully!";
                return RedirectToAction("Careers", "Home");
            }

            // Reload form with data
            var vacancy = _context.Vacancies.Find(hiring.VacancyId);
            ViewBag.ServiceTitle = vacancy?.Service?.Title;
            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            return View(hiring);
        }
    }
}