using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Category { get; set; } // e.g., "Manned Guarding", "Cash Services", etc.
    }
}