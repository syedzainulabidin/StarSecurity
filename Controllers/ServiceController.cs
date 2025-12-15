using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Authorize(Roles = "admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/services
        public IActionResult Index()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        // GET: /dashboard/services/create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /dashboard/services/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Service service)
        {
            ModelState.Remove("Employees");
            ModelState.Remove("Vacancies");
            ModelState.Remove("Bookings");

            if (ModelState.IsValid)
            {
                service.CreatedAt = DateTime.Now;
                service.UpdatedAt = DateTime.Now;

                _context.Services.Add(service);
                _context.SaveChanges();

                TempData["Message"] = "Service created successfully.";
                return RedirectToAction("Index");
            }
            return View(service);
        }

        // GET: /dashboard/services/edit/5
        public IActionResult Edit(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();
            return View(service);
        }

        // POST: /dashboard/services/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Service updatedService)
        {
            ModelState.Remove("Employees");
            ModelState.Remove("Vacancies");
            ModelState.Remove("Bookings");

            if (ModelState.IsValid)
            {
                var existing = _context.Services.Find(id);
                if (existing == null) return NotFound();

                existing.Title = updatedService.Title;
                existing.Description = updatedService.Description;
                existing.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                TempData["Message"] = "Service updated successfully.";
                return RedirectToAction("Index");
            }
            return View(updatedService);
        }

        // POST: /dashboard/services/delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();

            _context.Services.Remove(service);
            _context.SaveChanges();

            TempData["Message"] = "Service deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}