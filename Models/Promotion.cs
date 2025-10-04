using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangBanLe.Models
{
    public class Promotion : IValidatableObject
    {
        public int PromoId { get; set; }

        [Required(ErrorMessage = "Mã khuyến mãi là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Mã khuyến mãi phải có từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Mã khuyến mãi chỉ được chứa chữ in hoa, số, gạch ngang và gạch dưới")]
        [Display(Name = "Mã khuyến mãi")]
        public string PromoCode { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [RegularExpression(@"^(percent|fixed)$", ErrorMessage = "Loại giảm giá phải là 'percent' hoặc 'fixed'")]
        [Display(Name = "Loại giảm giá")]
        public string DiscountType { get; set; } = null!; // percent or fixed

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0.01, 100000000, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        [Display(Name = "Giá trị giảm giá")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Range(0, 999999999, ErrorMessage = "Giá trị đơn hàng tối thiểu phải từ 0 đến 999,999,999")]
        [Display(Name = "Giá trị đơn hàng tối thiểu")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sử dụng tối đa phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giới hạn sử dụng")]
        public int UsageLimit { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Số lần đã sử dụng không hợp lệ")]
        [Display(Name = "Đã sử dụng")]
        public int UsedCount { get; set; } = 0;

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [RegularExpression(@"^(active|inactive)$", ErrorMessage = "Trạng thái phải là 'active' hoặc 'inactive'")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "active"; // active or inactive

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Kiểm tra ngày kết thúc phải sau ngày bắt đầu
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc phải sau ngày bắt đầu",
                    new[] { nameof(EndDate) }
                );
            }

            // Kiểm tra giá trị giảm giá nếu là phần trăm
            if (DiscountType == "percent" && DiscountValue > 100)
            {
                yield return new ValidationResult(
                    "Giá trị giảm giá theo phần trăm không được vượt quá 100%",
                    new[] { nameof(DiscountValue) }
                );
            }

            // Kiểm tra số lần sử dụng
            if (UsedCount > UsageLimit && UsageLimit > 0)
            {
                yield return new ValidationResult(
                    "Số lần đã sử dụng không được vượt quá giới hạn",
                    new[] { nameof(UsedCount) }
                );
            }
        }

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
