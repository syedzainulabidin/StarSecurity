using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Authorize(Roles = "admin")]
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var clients = _context.Clients
                .Include(c => c.Booking)
                .ThenInclude(b => b.Service)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            return View(clients);
        }

        public IActionResult Create()
        {
            var completedBookings = _context.Bookings
                .Where(b => b.Status == "Completed")
                .Include(b => b.Service)
                .ToList();
            ViewBag.Bookings = new SelectList(completedBookings, "Id", "ClientName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Client client)
        {
            ModelState.Remove("Booking");

            if (ModelState.IsValid)
            {
                client.CreatedAt = DateTime.Now;
                client.UpdatedAt = DateTime.Now;

                _context.Clients.Add(client);
                _context.SaveChanges();

                TempData["Message"] = "Client added successfully.";
                return RedirectToAction("Index");
            }

            var completedBookings = _context.Bookings
                .Where(b => b.Status == "Completed")
                .Include(b => b.Service)
                .ToList();
            ViewBag.Bookings = new SelectList(completedBookings, "Id", "ClientName");
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null) return NotFound();

            _context.Clients.Remove(client);
            _context.SaveChanges();

            TempData["Message"] = "Client removed.";
            return RedirectToAction("Index");
        }
    }
}