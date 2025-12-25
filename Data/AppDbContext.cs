using Microsoft.EntityFrameworkCore;
using StarSecurity.Models;

namespace StarSecurity.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Hiring> Hirings { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();
        }
    }
}