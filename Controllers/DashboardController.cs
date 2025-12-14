using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;
using System.Security.Claims;

namespace StarSecurity.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard
        public IActionResult Index()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            ViewBag.Role = role;
            ViewBag.EmployeeName = User.FindFirst("Name")?.Value;

            if (role == "admin")
            {
                ViewBag.TotalEmployees = _context.Employees.Count();
                ViewBag.TotalBookings = _context.Bookings.Count();
                ViewBag.PendingApplications = _context.Hirings.Count(h => h.Status == "Pending");
                return View("AdminDashboard");
            }
            else
            {
                var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
                var assignedBookings = _context.Bookings
                    .Where(b => b.EmployeeId == empId && b.Status == "Approved")
                    .Count();
                ViewBag.AssignedBookings = assignedBookings;
                return View("StaffDashboard");
            }
        }

        // Staff: View own profile
        public IActionResult Profile()
        {
            var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var employee = _context.Employees
                .Include(e => e.Qualification)
                .Include(e => e.Service)
                .FirstOrDefault(e => e.Id == empId);

            if (employee == null) return NotFound();
            return View(employee);
        }

        // Staff: View assigned bookings (rename to MyBookings)
        public IActionResult MyBookings()
        {
            var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var bookings = _context.Bookings
                .Include(b => b.Service)
                .Where(b => b.EmployeeId == empId)
                .ToList();
            return View(bookings);
        }

        // Staff & Admin: View all employees (read-only for staff)
        public IActionResult Employees()
        {
            var employees = _context.Employees
                .Include(e => e.Qualification)
                .Include(e => e.Service)
                .ToList();
            return View(employees);
        }

        // ADMIN: Manage all bookings
        public IActionResult Bookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Employee)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            ViewBag.Employees = _context.Employees
                .Include(e => e.Service)
                .Where(e => e.Role == "staff")
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveBooking(int id, int employeeId)
        {
            if (employeeId == 0)
            {
                TempData["Error"] = "Please select a staff member.";
                return RedirectToAction("Bookings");
            }

            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Bookings");
            }

            booking.Status = "Approved";
            booking.EmployeeId = employeeId;
            booking.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Message"] = "Booking approved and staff assigned.";
            return RedirectToAction("Bookings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Bookings");
            }

            booking.Status = "Rejected";
            booking.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Message"] = "Booking rejected.";
            return RedirectToAction("Bookings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompleteBooking(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Bookings");
            }

            booking.Status = "Completed";
            booking.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Message"] = "Booking marked completed.";
            return RedirectToAction("Bookings");
        }
    }
}