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
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/requests
        [HttpGet("dashboard/requests")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Requests.OrderByDescending(r => r.SubmittedAt).ToListAsync());
        }

        // GET: /dashboard/requests/details/5
        [HttpGet("dashboard/requests/details/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: /request-security
        [HttpGet("request-security")]
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /request-security
        [HttpPost("request-security")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,ServiceNeeded,Description")] Request request)
        {
            if (ModelState.IsValid)
            {
                request.SubmittedAt = DateTime.Now; // Auto-set date
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home"); // Redirect to home after submission
            }
            return View(request);
        }

        // GET: /dashboard/requests/edit/5
        [HttpGet("dashboard/requests/edit/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        // POST: /dashboard/requests/edit/5
        [HttpPost("dashboard/requests/edit/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,ServiceNeeded,Description,SubmittedAt")] Request request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
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
            return View(request);
        }

        // GET: /dashboard/requests/delete/5
        [HttpGet("dashboard/requests/delete/{id}")]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: /dashboard/requests/delete/5
        [HttpPost("dashboard/requests/delete/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}