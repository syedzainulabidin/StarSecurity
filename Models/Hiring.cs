using System;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Hiring
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Contact { get; set; }

        public string Address { get; set; }

        public int? QualificationId { get; set; }
        public Qualification Qualification { get; set; }

        public int VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Reviewed, Rejected, Hired

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}