using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Authorize(Roles = "admin")]
    public class VacancyController : Controller
    {
        private readonly AppDbContext _context;

        public VacancyController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vacancies = _context.Vacancies
                .Include(v => v.Service)
                .OrderByDescending(v => v.PostedDate)
                .ToList();
            return View(vacancies);
        }

        public IActionResult Create()
        {
            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vacancy vacancy)
        {
            ModelState.Remove("Service");
            ModelState.Remove("Hirings");

            if (ModelState.IsValid)
            {
                vacancy.Status = "Open";
                vacancy.PostedDate = DateTime.Now;
                vacancy.CreatedAt = DateTime.Now;
                vacancy.UpdatedAt = DateTime.Now;

                _context.Vacancies.Add(vacancy);
                _context.SaveChanges();

                TempData["Message"] = "Vacancy posted successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View(vacancy);
        }

        public IActionResult Edit(int id)
        {
            var vacancy = _context.Vacancies.Find(id);
            if (vacancy == null) return NotFound();

            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View(vacancy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Vacancy updatedVacancy)
        {
            ModelState.Remove("Service");
            ModelState.Remove("Hirings");

            if (ModelState.IsValid)
            {
                var existing = _context.Vacancies.Find(id);
                if (existing == null) return NotFound();

                existing.ServiceId = updatedVacancy.ServiceId;
                existing.Count = updatedVacancy.Count;
                existing.Status = updatedVacancy.Status;
                existing.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                TempData["Message"] = "Vacancy updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View(updatedVacancy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var vacancy = _context.Vacancies.Find(id);
            if (vacancy == null) return NotFound();

            _context.Vacancies.Remove(vacancy);
            _context.SaveChanges();

            TempData["Message"] = "Vacancy deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}