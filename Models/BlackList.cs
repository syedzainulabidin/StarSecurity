using System;
using System.ComponentModel.DataAnnotations;

namespace StarSecurity.Models
{
    public class BlackList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Reason { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}