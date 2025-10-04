using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục phải có từ 2 đến 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; } = null!;

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
