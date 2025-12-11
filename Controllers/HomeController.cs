using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Helpers;
using StarSecurity.Models;

namespace StarSecurity.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("about")]
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("services")]
        [AllowAnonymous]
        public IActionResult Services()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        [HttpGet("contact")]
        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        //[HttpGet("careers")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Vacancies()
        //{
        //    var vacancies = await _context.Vacancies.Where(v => v.IsActive).ToListAsync();
        //    return View(vacancies);
        //}

        [HttpGet("dashboard")]
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

        [HttpGet("dashboard/admin")]
        [Helpers.Authorize("Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [HttpGet("dashboard/employee")]
        [Helpers.Authorize("Employee")]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }
    }
}