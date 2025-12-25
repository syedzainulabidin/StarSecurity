using System;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StarSecurity.Models
{
    public class Vacancy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required]
        public int Count { get; set; }

        public string Status { get; set; } = "Open";

        public DateTime PostedDate { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Hiring> Hirings { get; set; }
    }
}