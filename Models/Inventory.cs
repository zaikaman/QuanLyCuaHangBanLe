using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 0;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Product? Product { get; set; }
    }
}
