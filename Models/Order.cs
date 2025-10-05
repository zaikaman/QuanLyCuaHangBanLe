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

        [Required(ErrorMessage = "Ngày đặt hàng là bắt buộc")]
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Trạng thái đơn hàng là bắt buộc")]
        [RegularExpression(@"^(pending|paid|canceled)$", ErrorMessage = "Trạng thái không hợp lệ")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "pending"; // pending, paid, canceled

        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [Range(0, 999999999.99, ErrorMessage = "Tổng tiền phải từ 0 đến 999,999,999.99")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Số tiền giảm giá phải từ 0 đến 999,999,999.99")]
        [Display(Name = "Giảm giá")]
        public decimal DiscountAmount { get; set; } = 0;

        // Các thuộc tính điều hướng
        public Customer? Customer { get; set; }
        public User? User { get; set; }
        public Promotion? Promotion { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
