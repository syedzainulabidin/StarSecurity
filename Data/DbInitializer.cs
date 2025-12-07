using StarSecurity.Data;
using StarSecurity.Models;

namespace StarSecurity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Add Services
            if (!context.Services.Any())
            {
                context.Services.AddRange(
                    new Service { Name = "Manned Guarding", Description = "24/7 security personnel", Category = "Physical Security" },
                    new Service { Name = "Cash Services", Description = "Secure cash transportation", Category = "Financial Security" },
                    new Service { Name = "Electronic Security", Description = "CCTV, Alarm Systems", Category = "Technical Security" }
                );
                context.SaveChanges();
            }

            // Add Admin User
            if (!context.Users.Any(u => u.Email == "admin@starsecurity.com"))
            {
                var admin = new User
                {
                    Email = "admin@starsecurity.com",
                    PasswordHash = "admin123", // In production, hash passwords
                    FullName = "Administrator",
                    Role = "Admin",
                    Phone = "1234567890",
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
    }
}