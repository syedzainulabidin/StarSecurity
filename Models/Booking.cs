using System;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClientName { get; set; }

        [Required, EmailAddress]
        public string ClientEmail { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string ShiftStart { get; set; }

        [Required]
        public string ShiftEnd { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}