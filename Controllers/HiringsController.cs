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
    public class HiringsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HiringsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/applications
        [HttpGet("dashboard/applications")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hirings.ToListAsync());
        }

        // GET: /dashboard/applications/details/5
        [HttpGet("dashboard/applications/details/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hiring = await _context.Hirings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hiring == null)
            {
                return NotFound();
            }

            return View(hiring);
        }

        // GET: /apply-now
        [HttpGet("apply-now")]
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /apply-now
        [HttpPost("apply-now")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,ServiceInterested,Description")] Hiring hiring)
        {
            if (ModelState.IsValid)
            {
                hiring.SubmittedAt = DateTime.Now; // Auto-set date
                _context.Add(hiring);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home"); // Redirect to home after submission
            }
            return View(hiring);
        }

        // GET: /dashboard/applications/edit/5
        [HttpGet("dashboard/applications/edit/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hiring = await _context.Hirings.FindAsync(id);
            if (hiring == null)
            {
                return NotFound();
            }
            return View(hiring);
        }

        // POST: /dashboard/applications/edit/5
        [HttpPost("dashboard/applications/edit/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,ServiceInterested,Description,SubmittedAt")] Hiring hiring)
        {
            if (id != hiring.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hiring);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HiringExists(hiring.Id))
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
            return View(hiring);
        }

        // GET: /dashboard/applications/delete/5
        [HttpGet("dashboard/applications/delete/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hiring = await _context.Hirings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hiring == null)
            {
                return NotFound();
            }

            return View(hiring);
        }

        // POST: /dashboard/applications/delete/5
        [HttpPost("dashboard/applications/delete/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hiring = await _context.Hirings.FindAsync(id);
            if (hiring != null)
            {
                _context.Hirings.Remove(hiring);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HiringExists(int id)
        {
            return _context.Hirings.Any(e => e.Id == id);
        }
    }
}