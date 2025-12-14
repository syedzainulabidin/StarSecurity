using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StarSecurity.Models
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Degree { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Hiring> Hirings { get; set; }
    }
}