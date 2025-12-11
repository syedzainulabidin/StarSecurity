using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Helpers;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Route("dashboard/bookings")]
    [Helpers.Authorize("Admin", "Employee")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/bookings
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            if (userRole == "Employee")
            {
                // Employees only see their own bookings
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
                if (employee == null) return Forbid();

                var employeeBookings = _context.Bookings
                    .Include(b => b.Client)
                    .Include(b => b.Employee)
                    .Include(b => b.Service)
                    .Where(b => b.EmployeeId == employee.Id);

                return View(await employeeBookings.ToListAsync());
            }
            else
            {
                // Admins see all bookings
                var applicationDbContext = _context.Bookings
                    .Include(b => b.Client)
                    .Include(b => b.Employee)
                    .Include(b => b.Service);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: /dashboard/bookings/details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Employee)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            // Check if employee is viewing their own booking
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Employee")
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
                if (employee == null || booking.EmployeeId != employee.Id)
                    return RedirectToAction("AccessDenied", "Account");
            }

            return View(booking);
        }

        // GET: /dashboard/bookings/create
        [HttpGet("create")]
        [Helpers.Authorize("Admin")] // Only admin can create bookings
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Users.Where(u => u.Role == "Client"), "Id", "FullName");
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(e => e.User), "Id", "User.FullName");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        // POST: /dashboard/bookings/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Create([Bind("Id,ClientId,ServiceId,EmployeeId,Status")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.BookedAt = DateTime.Now; // Auto-set date
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Users.Where(u => u.Role == "Client"), "Id", "FullName", booking.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(e => e.User), "Id", "User.FullName", booking.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", booking.ServiceId);
            return View(booking);
        }

        // GET: /dashboard/bookings/edit/5
        [HttpGet("edit/{id}")]
        [Helpers.Authorize("Admin")] // Only admin can edit bookings
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] = new SelectList(_context.Users.Where(u => u.Role == "Client"), "Id", "FullName", booking.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(e => e.User), "Id", "User.FullName", booking.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", booking.ServiceId);
            return View(booking);
        }

        // POST: /dashboard/bookings/edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,ServiceId,EmployeeId,Status,BookedAt")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Users.Where(u => u.Role == "Client"), "Id", "FullName", booking.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(e => e.User), "Id", "User.FullName", booking.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", booking.ServiceId);
            return View(booking);
        }

        // GET: /dashboard/bookings/delete/5
        [HttpGet("delete/{id}")]
        [Helpers.Authorize("Admin")] // Only admin can delete bookings
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Employee)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: /dashboard/bookings/delete/5
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        [Helpers.Authorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}