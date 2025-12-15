using StarSecurity.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace StarSecurity.Data
{
    public static class Seeder
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Services.Any())
            {
                // 1. Services
                var services = new List<Service>
                {
                    new Service { Title = "Manned Guarding", Description = "Security personnel for sites" },
                    new Service { Title = "Cash Services", Description = "Secured cash transfer & ATM replenishment" },
                    new Service { Title = "Electronic Security", Description = "CCTV, Access Control, Alarms" },
                    new Service { Title = "Recruitment & Training", Description = "Manpower recruitment & training" }
                };
                context.Services.AddRange(services);
                context.SaveChanges();
            }

            if (!context.Qualifications.Any())
            {
                // 2. Qualifications
                var qualifications = new List<Qualification>
                {
                    new Qualification { Degree = "High School" },
                    new Qualification { Degree = "Diploma" },
                    new Qualification { Degree = "Bachelor's Degree" },
                    new Qualification { Degree = "Master's Degree" },
                    new Qualification { Degree = "Not Applicable" }
                };
                context.Qualifications.AddRange(qualifications);
                context.SaveChanges();
            }

            if (!context.Employees.Any())
            {
                // 3. Employees (Admin + Staff)
                var employees = new List<Employee>
                {
                    new Employee
                    {
                        Name = "Admin User",
                        Email = "admin@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Contact = "9876543210",
                        Address = "Mumbai HQ",
                        QualificationId = 5, // Not Applicable
                        ServiceId = 1,
                        Grade = "N/A",
                        Role = "admin"
                    },
                    new Employee
                    {
                        Name = "John Doe",
                        Email = "john@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("staff123"),
                        Contact = "9123456780",
                        Address = "Delhi",
                        QualificationId = 3,
                        ServiceId = 1,
                        Grade = "Senior Guard",
                        Role = "staff"
                    },
                    new Employee
                    {
                        Name = "Jane Smith",
                        Email = "jane@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("staff123"),
                        Contact = "9234567890",
                        Address = "Chennai",
                        QualificationId = 2,
                        ServiceId = 2,
                        Grade = "Cash Handler",
                        Role = "staff"
                    }
                };
                context.Employees.AddRange(employees);
                context.SaveChanges();
            }

            if (!context.Vacancies.Any())
            {
                // 4. Vacancies
                var vacancies = new List<Vacancy>
                {
                    new Vacancy { ServiceId = 1, Count = 5, Status = "Open", PostedDate = DateTime.Now.AddDays(-10) },
                    new Vacancy { ServiceId = 2, Count = 3, Status = "Open", PostedDate = DateTime.Now.AddDays(-5) },
                    new Vacancy { ServiceId = 3, Count = 2, Status = "Closed", PostedDate = DateTime.Now.AddDays(-30) }
                };
                context.Vacancies.AddRange(vacancies);
                context.SaveChanges();
            }

            if (!context.Bookings.Any())
            {
                // 5. Bookings
                var bookings = new List<Booking>
                {
                    new Booking
                    {
                        ClientName = "ABC Corporation",
                        ClientEmail = "contact@abccorp.com",
                        Description = "Need security for corporate office",
                        Date = DateTime.Now.AddDays(7),
                        ShiftStart = "02:00",
                        ShiftEnd = "04:00",
                        ServiceId = 1,
                        EmployeeId = 2, // John Doe assigned
                        Address = "Mumbai",
                        Status = "Approved"
                    },
                    new Booking
                    {
                        ClientName = "XYZ Bank",
                        ClientEmail = "security@xyzbank.com",
                        Description = "ATM cash replenishment",
                        Date = DateTime.Now.AddDays(3),
                        ShiftStart = "00:00",
                        ShiftEnd = "06:00",
                        ServiceId = 2,
                        Address = "Delhi",
                        Status = "Pending"
                    }
                };
                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }

            if (!context.Clients.Any())
            {
                // 6. Clients (from completed booking)
                var clients = new List<Client>
                {
                    new Client { BookingId = 1 }
                };
                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

            if (!context.Testimonials.Any())
            {
                // 7. Testimonials
                var testimonials = new List<Testimonial>
                {
                    new Testimonial { ClientId = 1, Content = "Excellent service, highly professional!", Rating = 5 }
                };
                context.Testimonials.AddRange(testimonials);
                context.SaveChanges();
            }

            if (!context.Hirings.Any())
            {
                // Get the first existing vacancy
                var vacancy = context.Vacancies.FirstOrDefault();
                if (vacancy != null)
                {
                    var hirings = new List<Hiring>
        {
            new Hiring
            {
                Name = "Rohit Sharma",
                Email = "rohit@gmail.com",
                Contact = "9123401234",
                Address = "Pune",
                QualificationId = 3,
                VacancyId = vacancy.Id, // Use actual existing ID
                Status = "Pending"
            }
        };
                    context.Hirings.AddRange(hirings);
                    context.SaveChanges();
                }
            }
        }
    }
}