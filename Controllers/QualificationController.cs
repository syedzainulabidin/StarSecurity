using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Authorize(Roles = "admin")]
    public class QualificationController : Controller
    {
        private readonly AppDbContext _context;

        public QualificationController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var qualifications = _context.Qualifications.ToList();
            return View(qualifications);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Qualification qualification)
        {
            ModelState.Remove("Employees");
            ModelState.Remove("Hirings");

            if (ModelState.IsValid)
            {
                qualification.CreatedAt = DateTime.Now;
                qualification.UpdatedAt = DateTime.Now;

                _context.Qualifications.Add(qualification);
                _context.SaveChanges();

                TempData["Message"] = "Qualification created successfully.";
                return RedirectToAction("Index");
            }

            return View(qualification);
        }

        public IActionResult Edit(int id)
        {
            var qualification = _context.Qualifications.Find(id);
            if (qualification == null) return NotFound();

            return View(qualification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Qualification updatedQualification)
        {
            ModelState.Remove("Employees");
            ModelState.Remove("Hirings");

            if (ModelState.IsValid)
            {
                var existing = _context.Qualifications.Find(id);
                if (existing == null) return NotFound();

                existing.Degree = updatedQualification.Degree;
                existing.UpdatedAt = DateTime.Now;

                _context.SaveChanges();

                TempData["Message"] = "Qualification updated successfully.";
                return RedirectToAction("Index");
            }

            return View(updatedQualification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var qualification = _context.Qualifications
                .Include(q => q.Employees)
                .Include(q => q.Hirings)
                .FirstOrDefault(q => q.Id == id);

            if (qualification == null)
                return NotFound();

            if (qualification.Employees.Any() || qualification.Hirings.Any())
            {
                TempData["Message"] =
                    "Cannot delete qualification because it is used by employees or hirings.";
                return RedirectToAction("Index");
            }

            _context.Qualifications.Remove(qualification);
            _context.SaveChanges();

            TempData["Message"] = "Qualification deleted successfully.";
            return RedirectToAction("Index");
        }


    }
}
