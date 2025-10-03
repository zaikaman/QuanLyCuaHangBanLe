using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Promotion
    {
        public int PromoId { get; set; }

        [Required]
        [StringLength(50)]
        public string PromoCode { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        public string DiscountType { get; set; } = null!; // percent or fixed

        [Required]
        public decimal DiscountValue { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public decimal MinOrderAmount { get; set; } = 0;

        public int UsageLimit { get; set; } = 0;

        public int UsedCount { get; set; } = 0;

        public string Status { get; set; } = "active"; // active or inactive
    }
}
