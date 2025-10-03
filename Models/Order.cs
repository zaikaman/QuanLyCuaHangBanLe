using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int? CustomerId { get; set; }

        public int? UserId { get; set; }

        public int? PromoId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "pending"; // pending, paid, canceled

        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; } = 0;

        // Navigation properties
        public Customer? Customer { get; set; }
        public User? User { get; set; }
        public Promotion? Promotion { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
