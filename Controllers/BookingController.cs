using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Book(int serviceId)
        {
            var service = _context.Services.Find(serviceId);
            if (service == null) return NotFound();

            ViewBag.ServiceTitle = service.Title;
            ViewBag.ServiceId = serviceId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(Booking booking)
        {
            ModelState.Remove("Service");
            ModelState.Remove("Employee");

            var existing = _context.Bookings
                .Where(b => b.ClientEmail == booking.ClientEmail)
                .Where(b => b.Status == "Pending" || b.Status == "Approved")
                .Where(b => b.Date >= DateTime.Now.Date)
                .FirstOrDefault();

            if (existing != null)
            {
                ModelState.AddModelError("ClientEmail", "You already have an active booking.");
            }

            if (ModelState.IsValid)
            {
                booking.Status = "Pending";
                booking.CreatedAt = DateTime.Now;
                booking.UpdatedAt = DateTime.Now;

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                TempData["Success"] = "Booking request submitted successfully!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ServiceTitle = _context.Services.Find(booking.ServiceId)?.Title;
            ViewBag.ServiceId = booking.ServiceId;
            return View(booking);
        }


    }
}