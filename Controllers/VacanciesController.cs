using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;
using StarSecurity.Helpers;

namespace StarSecurity.Controllers
{
    [Route("")]
    [Helpers.Authorize("Admin")]
    public class VacanciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VacanciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/vacancies (Admin view - all vacancies)
        [HttpGet("dashboard/vacancies")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vacancies.ToListAsync());
        }

        // GET: /careers (Public view - active vacancies only)
        [HttpGet("careers")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicIndex()
        {
            var vacancies = await _context.Vacancies.Where(v => v.IsActive).ToListAsync();
            return View("PublicIndex", vacancies); // Different view for public
        }

        // GET: /dashboard/vacancies/details/5
        [HttpGet("dashboard/vacancies/details/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        // GET: /dashboard/vacancies/create
        [HttpGet("dashboard/vacancies/create")]
        [Helpers.Authorize("Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /dashboard/vacancies/create
        [HttpPost("dashboard/vacancies/create")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Count,IsActive")] Vacancy vacancy)
        {
            if (ModelState.IsValid)
            {
                vacancy.PostedAt = DateTime.Now; // Auto-set date
                _context.Add(vacancy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vacancy);
        }

        // GET: /dashboard/vacancies/edit/5
        [HttpGet("dashboard/vacancies/edit/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }
            return View(vacancy);
        }

        // POST: /dashboard/vacancies/edit/5
        [HttpPost("dashboard/vacancies/edit/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Count,IsActive,PostedAt")] Vacancy vacancy)
        {
            if (id != vacancy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacancy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacancyExists(vacancy.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vacancy);
        }

        // GET: /dashboard/vacancies/delete/5
        [HttpGet("dashboard/vacancies/delete/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        // POST: /dashboard/vacancies/delete/5
        [HttpPost("dashboard/vacancies/delete/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy != null)
            {
                _context.Vacancies.Remove(vacancy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacancyExists(int id)
        {
            return _context.Vacancies.Any(e => e.Id == id);
        }
    }
}