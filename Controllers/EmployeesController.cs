using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Helpers;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Route("dashboard/employees")]
    [Helpers.Authorize("Admin", "Employee")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/employees
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "Employee")
            {
                // Employees see all colleagues (similar to ViewColleagues but with less detail)
                var currentUserId = int.Parse(HttpContext.Session.GetString("UserId"));
                var colleagues = await _context.Employees
                    .Include(e => e.User)
                    .Include(e => e.Service)
                    .Where(e => e.UserId != currentUserId)
                    .ToListAsync();
                return View(colleagues);
            }
            else
            {
                // Admins see all employees with full management
                var applicationDbContext = _context.Employees
                    .Include(e => e.Service)
                    .Include(e => e.User);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: /dashboard/employees/details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Service)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            // Employees can view any colleague's details
            return View(employee);
        }

        // GET: /dashboard/employees/create
        [HttpGet("create")]
        [Helpers.Authorize("Admin")] // Only admin can create employees
        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == "Employee" && !_context.Employees.Any(e => e.UserId == u.Id)), "Id", "FullName");
            return View();
        }

        // POST: /dashboard/employees/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Create([Bind("Id,UserId,ServiceId,Rating")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", employee.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == "Employee"), "Id", "FullName", employee.UserId);
            return View(employee);
        }

        // GET: /dashboard/employees/edit/5
        [HttpGet("edit/{id}")]
        [Helpers.Authorize("Admin")] // Only admin can edit employees
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", employee.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == "Employee"), "Id", "FullName", employee.UserId);
            return View(employee);
        }

        // POST: /dashboard/employees/edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ServiceId,Rating")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", employee.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == "Employee"), "Id", "FullName", employee.UserId);
            return View(employee);
        }

        // GET: /dashboard/employees/delete/5
        [HttpGet("delete/{id}")]
        [Helpers.Authorize("Admin")] // Only admin can delete employees
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Service)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: /dashboard/employees/delete/5
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}