using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải có từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số, gạch ngang và gạch dưới")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải có từ 2 đến 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [RegularExpression(@"^(admin|staff)$", ErrorMessage = "Vai trò phải là 'admin' hoặc 'staff'")]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "staff"; // admin or staff

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
