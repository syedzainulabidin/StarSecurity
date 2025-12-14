using Microsoft.AspNetCore.Mvc;
using StarSecurity.Data;
using StarSecurity.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace StarSecurity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        public IActionResult Network()
        {
            return View();
        }

        public IActionResult Careers()
        {
            var vacancies = _context.Vacancies
                .Include(v => v.Service)
                .Where(v => v.Status == "Open")
                .ToList();
            return View(vacancies);
        }

        public IActionResult Testimonials()
        {
            var testimonials = _context.Testimonials
                .Include(t => t.Client)
                .ThenInclude(c => c.Booking)
                .ToList();
            return View(testimonials);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}