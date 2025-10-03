using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        // Navigation properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
