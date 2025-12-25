using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;
                _context.Contacts.Add(contact);
                _context.SaveChanges();

                TempData["Success"] = "Your message has been sent. Thank you!";
                return RedirectToAction("Index");
            }
            return View("Index", contact);
        }

        [Authorize(Roles = "admin")]
        public IActionResult ContactInquiries()
        {
            var inquiries = _context.Contacts
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            return View("~/Views/Dashboard/ContactInquiries.cshtml", inquiries);
        }
    }
}