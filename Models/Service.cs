using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Vacancy> Vacancies { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}