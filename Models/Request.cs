using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarSecurity.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ServiceNeeded { get; set; }

        public string Description { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}