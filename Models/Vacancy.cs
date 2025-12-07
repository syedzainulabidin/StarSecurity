using System;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Vacancy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int Count { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public DateTime PostedAt { get; set; } = DateTime.Now;
    }
}