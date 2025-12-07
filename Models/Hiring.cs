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

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ServiceInterested { get; set; }

        public string Description { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}