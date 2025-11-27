using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải có từ 2 đến 100 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Mã vạch không được vượt quá 50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9-_]*$", ErrorMessage = "Mã vạch chỉ được chứa chữ cái, số, dấu gạch ngang và gạch dưới")]
        [Display(Name = "Mã vạch")]
        public string? Barcode { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0.01, 999999999.99, ErrorMessage = "Giá sản phẩm phải lớn hơn 0 và không vượt quá 999,999,999.99")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Đơn vị tính là bắt buộc")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Đơn vị tính phải có từ 1 đến 20 ký tự")]
        [Display(Name = "Đơn vị tính")]
        public string Unit { get; set; } = "pcs";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ảnh sản phẩm")]
        public string? ImageUrl { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }
        public Inventory? Inventory { get; set; }
    }
}
