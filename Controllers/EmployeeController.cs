using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;
using StarSecurity.Helpers;

namespace StarSecurity.Controllers
{
    [Helpers.Authorize("Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ViewColleagues()
        {
            var currentUserId = int.Parse(HttpContext.Session.GetString("UserId"));
            var colleagues = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Service)
                .Where(e => e.UserId != currentUserId)
                .ToListAsync();

            return View(colleagues);
        }

        public async Task<IActionResult> MyProfile()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Service)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                return NotFound();

            var bookings = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Service)
                .Where(b => b.EmployeeId == employee.Id)
                .ToListAsync();

            return View(bookings);
        }
    }
}