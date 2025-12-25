using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;
using System.Security.Claims;
using BCrypt.Net;

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

        public IActionResult Index()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            ViewBag.Role = role;
            ViewBag.EmployeeName = User.FindFirst("Name")?.Value;

            if (role == "admin")
            {
                ViewBag.TotalEmployees = _context.Employees.Count();
                ViewBag.TotalBookings = _context.Bookings.Count();
                ViewBag.PendingBookings = _context.Bookings.Count(b => b.Status == "Pending");
                ViewBag.ApprovedBookings = _context.Bookings.Count(b => b.Status == "Approved");
                ViewBag.CompletedBookings = _context.Bookings.Count(b => b.Status == "Completed");
                ViewBag.PendingApplications = _context.Hirings.Count(h => h.Status == "Pending");
                ViewBag.TotalClients = _context.Clients.Count();
                ViewBag.TotalTestimonials = _context.Testimonials.Count();

                ViewBag.RecentBookings = _context.Bookings
                    .Include(b => b.Service)
                    .Include(b => b.Employee)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(5)
                    .ToList();

                ViewBag.RecentApplications = _context.Hirings
                    .Include(h => h.Vacancy)
                        .ThenInclude(v => v.Service)
                    .OrderByDescending(h => h.CreatedAt)
                    .Take(5)
                    .ToList();

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

        public IActionResult Profile()
        {
            var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var employee = _context.Employees
                .Include(e => e.Qualification)
                .Include(e => e.Service)
                .FirstOrDefault(e => e.Id == empId);

            if (employee == null) return NotFound();

            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(Employee updatedEmployee)
        {
            var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var existing = _context.Employees.Find(empId);
            if (existing == null) return NotFound();

            ModelState.Remove("Password");
            ModelState.Remove("Email");
            ModelState.Remove("Role");
            ModelState.Remove("Qualification");
            ModelState.Remove("Service");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "admin")
            {
                ModelState.Remove("ServiceId");
                ModelState.Remove("Grade");
            }

            if (ModelState.IsValid)
            {
                existing.Name = updatedEmployee.Name;
                existing.Contact = updatedEmployee.Contact;
                existing.Address = updatedEmployee.Address;
                existing.QualificationId = updatedEmployee.QualificationId;
                existing.UpdatedAt = DateTime.Now;

                if (role == "admin")
                {
                    existing.ServiceId = updatedEmployee.ServiceId;
                    existing.Grade = updatedEmployee.Grade;
                }

                _context.SaveChanges();
                TempData["Message"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }

            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View("Profile", updatedEmployee);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var employee = _context.Employees.Find(empId);
            if (employee == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, employee.Password))
            {
                ModelState.AddModelError("currentPassword", "Current password is incorrect.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ModelState.AddModelError("newPassword", "New password must be at least 6 characters.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "New password and confirmation do not match.");
                return View();
            }

            employee.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            employee.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }

        public IActionResult MyBookings()
        {
            var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var bookings = _context.Bookings
                .Include(b => b.Service)
                .Where(b => b.EmployeeId == empId)
                .ToList();
            return View(bookings);
        }

        public IActionResult Employees()
        {
            var employees = _context.Employees
                .Include(e => e.Qualification)
                .Include(e => e.Service)
                .ToList();
            return View(employees);
        }

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "staff")]
        public IActionResult Colleagues()
        {
            var empId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var colleagues = _context.Employees
                .Include(e => e.Qualification)
                .Include(e => e.Service)
                .Where(e => e.Id != empId)
                .ToList();
            return View(colleagues);
        }
    }
}