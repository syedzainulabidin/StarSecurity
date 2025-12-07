using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Helpers;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Services()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Vacancies()
        {
            var vacancies = await _context.Vacancies.Where(v => v.IsActive).ToListAsync();
            return View(vacancies);
        }

        [Helpers.Authorize("Admin", "Employee")]
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Admin")
                return RedirectToAction("AdminDashboard");
            else if (role == "Employee")
                return RedirectToAction("EmployeeDashboard");

            return RedirectToAction("Index");
        }

        [Helpers.Authorize("Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Helpers.Authorize("Employee")]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }
    }
}