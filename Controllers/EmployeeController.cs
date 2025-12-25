using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarSecurity.Data;
using StarSecurity.Models;
using BCrypt.Net;

namespace StarSecurity.Controllers
{
    [Authorize(Roles = "admin")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var employees = _context.Employees
                .Include(e => e.Qualification)
                .Include(e => e.Service)
                .ToList();
            return View("~/Views/Employee/Index.cshtml", employees);
        }

        public IActionResult Create()
        {
            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            ModelState.Remove("Qualification");
            ModelState.Remove("Service");

            if (ModelState.IsValid)
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(employee.Password);
                employee.CreatedAt = DateTime.Now;
                employee.UpdatedAt = DateTime.Now;

                _context.Employees.Add(employee);
                _context.SaveChanges();

                TempData["Message"] = "Employee created successfully.";
                return RedirectToAction("Index");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["Error"] = string.Join("<br/>", errors);

            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View(employee);
        }

        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null) return NotFound();

            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee updatedEmployee)
        {
            ModelState.Remove("Qualification");
            ModelState.Remove("Service");
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var existing = _context.Employees.Find(id);
                if (existing == null) return NotFound();

                existing.Name = updatedEmployee.Name;
                existing.Email = updatedEmployee.Email;
                existing.Contact = updatedEmployee.Contact;
                existing.Address = updatedEmployee.Address;
                existing.QualificationId = updatedEmployee.QualificationId;
                existing.ServiceId = updatedEmployee.ServiceId;
                existing.Grade = updatedEmployee.Grade;
                existing.Role = updatedEmployee.Role;
                existing.UpdatedAt = DateTime.Now;

                if (!string.IsNullOrEmpty(updatedEmployee.Password))
                {
                    existing.Password = BCrypt.Net.BCrypt.HashPassword(updatedEmployee.Password);
                }

                _context.SaveChanges();
                TempData["Message"] = "Employee updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.Qualifications = new SelectList(_context.Qualifications, "Id", "Degree");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Title");
            return View(updatedEmployee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            TempData["Message"] = "Employee deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}