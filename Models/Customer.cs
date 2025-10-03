using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
