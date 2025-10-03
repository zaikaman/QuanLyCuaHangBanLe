using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = null!;

        [StringLength(50)]
        public string? Barcode { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(20)]
        public string Unit { get; set; } = "pcs";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
