using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Authorize(Roles = "admin")]
    public class TestimonialController : Controller
    {
        private readonly AppDbContext _context;

        public TestimonialController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /dashboard/testimonials
        public IActionResult Index()
        {
            var testimonials = _context.Testimonials
                .Include(t => t.Client)
                    .ThenInclude(c => c.Booking)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();
            return View(testimonials);
        }

        // GET: /dashboard/testimonials/create
        public IActionResult Create()
        {
            var clients = _context.Clients
                .Include(c => c.Booking)
                .Select(c => new
                {
                    Id = c.Id,
                    Display = c.Booking.ClientName + " (" + c.Booking.Service.Title + ")"
                })
                .ToList();
            ViewBag.Clients = new SelectList(clients, "Id", "Display");
            return View();
        }

        // POST: /dashboard/testimonials/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Testimonial testimonial)
        {
            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                testimonial.CreatedAt = DateTime.Now;
                testimonial.UpdatedAt = DateTime.Now;

                _context.Testimonials.Add(testimonial);
                _context.SaveChanges();

                TempData["Message"] = "Testimonial added successfully.";
                return RedirectToAction("Index");
            }

            var clients = _context.Clients
                .Include(c => c.Booking)
                .Select(c => new
                {
                    Id = c.Id,
                    Display = c.Booking.ClientName + " (" + c.Booking.Service.Title + ")"
                })
                .ToList();
            ViewBag.Clients = new SelectList(clients, "Id", "Display");
            return View(testimonial);
        }

        // GET: /dashboard/testimonials/edit/5
        public IActionResult Edit(int id)
        {
            var testimonial = _context.Testimonials.Find(id);
            if (testimonial == null) return NotFound();

            var clients = _context.Clients
                .Include(c => c.Booking)
                .Select(c => new
                {
                    Id = c.Id,
                    Display = c.Booking.ClientName + " (" + c.Booking.Service.Title + ")"
                })
                .ToList();
            ViewBag.Clients = new SelectList(clients, "Id", "Display");
            return View(testimonial);
        }

        // POST: /dashboard/testimonials/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Testimonial updatedTestimonial)
        {
            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                var existing = _context.Testimonials.Find(id);
                if (existing == null) return NotFound();

                existing.ClientId = updatedTestimonial.ClientId;
                existing.Content = updatedTestimonial.Content;
                existing.Rating = updatedTestimonial.Rating;
                existing.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                TempData["Message"] = "Testimonial updated.";
                return RedirectToAction("Index");
            }

            var clients = _context.Clients
                .Include(c => c.Booking)
                .Select(c => new
                {
                    Id = c.Id,
                    Display = c.Booking.ClientName + " (" + c.Booking.Service.Title + ")"
                })
                .ToList();
            ViewBag.Clients = new SelectList(clients, "Id", "Display");
            return View(updatedTestimonial);
        }

        // POST: /dashboard/testimonials/delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var testimonial = _context.Testimonials.Find(id);
            if (testimonial == null) return NotFound();

            _context.Testimonials.Remove(testimonial);
            _context.SaveChanges();

            TempData["Message"] = "Testimonial deleted.";
            return RedirectToAction("Index");
        }
    }
}