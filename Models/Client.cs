using System;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}