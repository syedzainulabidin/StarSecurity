using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;
using StarSecurity.Helpers;

namespace StarSecurity.Controllers
{
    [Route("dashboard/blacklist")]
    [Helpers.Authorize("Admin")]
    public class BlackListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlackListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/blacklist
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.BlackLists.ToListAsync());
        }

        // GET: /dashboard/blacklist/details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blackList = await _context.BlackLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blackList == null)
            {
                return NotFound();
            }

            return View(blackList);
        }

        // GET: /dashboard/blacklist/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /dashboard/blacklist/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Reason")] BlackList blackList)
        {
            if (ModelState.IsValid)
            {
                blackList.AddedAt = DateTime.Now; // Auto-set date
                _context.Add(blackList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blackList);
        }

        // GET: /dashboard/blacklist/edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blackList = await _context.BlackLists.FindAsync(id);
            if (blackList == null)
            {
                return NotFound();
            }
            return View(blackList);
        }

        // POST: /dashboard/blacklist/edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Reason,AddedAt")] BlackList blackList)
        {
            if (id != blackList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blackList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlackListExists(blackList.Id))
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
            return View(blackList);
        }

        // GET: /dashboard/blacklist/delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blackList = await _context.BlackLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blackList == null)
            {
                return NotFound();
            }

            return View(blackList);
        }

        // POST: /dashboard/blacklist/delete/5
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blackList = await _context.BlackLists.FindAsync(id);
            if (blackList != null)
            {
                _context.BlackLists.Remove(blackList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlackListExists(int id)
        {
            return _context.BlackLists.Any(e => e.Id == id);
        }
    }
}