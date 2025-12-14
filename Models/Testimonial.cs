using System;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [Required]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}