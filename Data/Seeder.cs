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
      
                var employees = new List<Employee>
                {
                    new Employee
                    {
                        Name = "Admin User",
                        Email = "admin@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Contact = "123456789",
                        Address = "N/A",
                        QualificationId = 5,
                        ServiceId = 1,
                        Grade = "N/A",
                        Role = "admin"
                    },
                     new Employee
                    {
                        Name = "Syed Zain ul Abidin",
                        Email = "zain@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("staff123"),
                        Contact = "123456789",
                        Address = "Karachi",
                        QualificationId = 2,
                        ServiceId = 2,
                        Grade = "Supervisor",
                        Role = "staff"
                    },
                    new Employee
                    {
                        Name = "Muhammad Hamza",
                        Email = "hamza@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("staff123"),
                        Contact = "9254687544",
                        Address = "Karachi",
                        QualificationId = 3,
                        ServiceId = 1,
                        Grade = "Senior Guard",
                        Role = "staff"
                    },
                    new Employee
                    {
                        Name = "Abdul Moiz",
                        Email = "moiz@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("staff123"),
                        Contact = "9234567890",
                        Address = "Peshawar",
                        QualificationId = 2,
                        ServiceId = 2,
                        Grade = "Cash Handler",
                        Role = "staff"
                    }
                    ,
                    new Employee
                    {
                        Name = "Ahmed Ali",
                        Email = "ahmed@starsecurity.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("staff123"),
                        Contact = "92326522490",
                        Address = "Lahore",
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
                var bookings = new List<Booking>
                {
                    new Booking
                    {
                        ClientName = "HBL",
                        ClientEmail = "hbl@gmail.com",
                        Description = "Need security for corporate office",
                        Date = DateTime.Now.AddDays(7),
                        ShiftStart = "02:00",
                        ShiftEnd = "04:00",
                        ServiceId = 1,
                        EmployeeId = 2,
                        Address = "Lyari, Karachi",
                        Status = "Approved"
                    },
                    new Booking
                    {
                        ClientName = "Allied Bank Limited",
                        ClientEmail = "allied@gmail.com",
                        Description = "ATM cash replenishment",
                        Date = DateTime.Now.AddDays(3),
                        ShiftStart = "00:00",
                        ShiftEnd = "06:00",
                        ServiceId = 2,
                        Address = "Saddar, Lahore",
                        Status = "Pending"
                    },
                    new Booking
                    {
                        ClientName = "TCL",
                        ClientEmail = "tcl@gmail.com",
                        Description = "Mobile Repair Shop Security Guarded Needed",
                        Date = DateTime.Now.AddDays(2),
                        ShiftStart = "01:00",
                        ShiftEnd = "08:00",
                        ServiceId = 1,
                        Address = "Korangi, Karachi",
                        Status = "Pending"
                    }
                };
                context.Bookings.AddRange(bookings);
                context.SaveChanges();
            }

            if (!context.Clients.Any())
            {
                var clients = new List<Client>
                {
                    new Client { BookingId = 1 }
                };
                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

            if (!context.Testimonials.Any())
            {
                var testimonials = new List<Testimonial>
                {
                    new Testimonial { ClientId = 1, Content = "Excellent service, highly professional!", Rating = 5 }

                };
                context.Testimonials.AddRange(testimonials);
                context.SaveChanges();
            }

            if (!context.Hirings.Any())
            {
                var vacancy = context.Vacancies.FirstOrDefault();
                if (vacancy != null)
                {
                    var hirings = new List<Hiring>
        {
            new Hiring
            {
                Name = "Shahzaib Ghayyas",
                Email = "shahzaib@gmail.com",
                Contact = "924684564",
                Address = "ShahFaisal",
                QualificationId = 3,
                VacancyId = vacancy.Id,
                Status = "Pending"
            },
             new Hiring
            {
                Name = "Bilal Fawad",
                Email = "bilal@gmail.com",
                Contact = "9223401234",
                Address = "Liaquatabad",
                QualificationId = 2,
                VacancyId = vacancy.Id,
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