using System;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        //[Required]
        public string Password { get; set; }

        [Required]
        public string Contact { get; set; }

        public string Address { get; set; }

        public int? QualificationId { get; set; }
        public Qualification Qualification { get; set; }

        public int? ServiceId { get; set; }
        public Service Service { get; set; }

        public string Grade { get; set; }

        [Required]
        public string Role { get; set; } = "staff"; // admin or staff

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}