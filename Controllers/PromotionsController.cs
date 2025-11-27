using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Filters;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    [AdminOnly]
    public class PromotionsController : Controller
    {
        private readonly IGenericRepository<Promotion> _promotionRepository;

        public PromotionsController(IGenericRepository<Promotion> promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            const int pageSize = 10;
            var allPromotions = await _promotionRepository.GetAllAsync();
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allPromotions = allPromotions.Where(p =>
                    (p.PromoCode != null && p.PromoCode.ToLower().Contains(searchTerm)) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    (p.Status != null && p.Status.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            var totalItems = allPromotions.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var promotions = allPromotions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(promotions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotion promotion)
        {
            if (promotion == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(promotion.PromoCode))
            {
                ModelState.AddModelError("PromoCode", "Mã khuyến mãi không được để trống");
            }

            // Kiểm tra ngày
            if (promotion.EndDate <= promotion.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
            }

            // Kiểm tra giá trị giảm giá theo phần trăm
            if (promotion.DiscountType == "percent" && promotion.DiscountValue > 100)
            {
                ModelState.AddModelError("DiscountValue", "Giá trị giảm giá theo phần trăm không được vượt quá 100%");
            }

            // Kiểm tra mã khuyến mãi trùng
            if (!string.IsNullOrWhiteSpace(promotion.PromoCode))
            {
                var existingPromotions = await _promotionRepository.GetAllAsync();
                if (existingPromotions.Any(p => p.PromoCode.Equals(promotion.PromoCode, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("PromoCode", "Mã khuyến mãi này đã tồn tại");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(promotion);
            }

            try
            {
                promotion.PromoCode = promotion.PromoCode.ToUpper().Trim();
                promotion.Description = promotion.Description?.Trim();
                
                await _promotionRepository.AddAsync(promotion);
                TempData["SuccessMessage"] = "Thêm khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi thêm khuyến mãi: {ex.Message}");
                
                // Kiểm tra lỗi duplicate key
                if (ex.InnerException?.Message.Contains("Duplicate entry") == true || 
                    ex.Message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("PromoCode", "Mã khuyến mãi này đã tồn tại");
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi thêm khuyến mãi: " + ex.Message);
                }
                
                return View(promotion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Promotion promotion)
        {
            if (promotion == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(promotion.PromoCode))
            {
                ModelState.AddModelError("PromoCode", "Mã khuyến mãi không được để trống");
            }

            // Kiểm tra ngày
            if (promotion.EndDate <= promotion.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
            }

            // Kiểm tra giá trị giảm giá theo phần trăm
            if (promotion.DiscountType == "percent" && promotion.DiscountValue > 100)
            {
                ModelState.AddModelError("DiscountValue", "Giá trị giảm giá theo phần trăm không được vượt quá 100%");
            }

            // Kiểm tra số lần sử dụng
            if (promotion.UsedCount > promotion.UsageLimit && promotion.UsageLimit > 0)
            {
                ModelState.AddModelError("UsedCount", "Số lần đã sử dụng không được vượt quá giới hạn");
            }

            // Kiểm tra mã khuyến mãi trùng (trừ chính nó)
            if (!string.IsNullOrWhiteSpace(promotion.PromoCode))
            {
                var allPromotions = await _promotionRepository.GetAllAsync();
                if (allPromotions.Any(p => p.PromoId != promotion.PromoId && 
                    p.PromoCode.Equals(promotion.PromoCode, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("PromoCode", "Mã khuyến mãi này đã tồn tại");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(promotion);
            }

            try
            {
                var existingPromotion = await _promotionRepository.GetByIdAsync(promotion.PromoId);
                if (existingPromotion == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khuyến mãi";
                    return RedirectToAction(nameof(Index));
                }

                existingPromotion.PromoCode = promotion.PromoCode.ToUpper().Trim();
                existingPromotion.Description = promotion.Description?.Trim();
                existingPromotion.DiscountType = promotion.DiscountType;
                existingPromotion.DiscountValue = promotion.DiscountValue;
                existingPromotion.StartDate = promotion.StartDate;
                existingPromotion.EndDate = promotion.EndDate;
                existingPromotion.MinOrderAmount = promotion.MinOrderAmount;
                existingPromotion.UsageLimit = promotion.UsageLimit;
                existingPromotion.Status = promotion.Status;

                await _promotionRepository.UpdateAsync(existingPromotion);
                TempData["SuccessMessage"] = "Cập nhật khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi cập nhật khuyến mãi: {ex.Message}");
                
                // Kiểm tra lỗi duplicate key
                if (ex.InnerException?.Message.Contains("Duplicate entry") == true || 
                    ex.Message.Contains("duplicate key"))
                {
                    ModelState.AddModelError("PromoCode", "Mã khuyến mãi này đã tồn tại");
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật khuyến mãi: " + ex.Message);
                }
                
                return View(promotion);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "ID không hợp lệ" });
                }

                var promotion = await _promotionRepository.GetByIdAsync(id);
                if (promotion == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khuyến mãi" });
                }

                // Kiểm tra xem khuyến mãi có đơn hàng nào đang sử dụng không
                if (promotion.Orders != null && promotion.Orders.Any())
                {
                    return Json(new { success = false, message = $"Không thể xóa khuyến mãi '{promotion.PromoCode}' vì còn {promotion.Orders.Count} đơn hàng đang sử dụng" });
                }

                await _promotionRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa khuyến mãi thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xóa khuyến mãi: {ex.Message}");
                
                // Kiểm tra lỗi foreign key constraint
                if (ex.InnerException?.Message.Contains("foreign key constraint") == true || 
                    ex.Message.Contains("FOREIGN KEY") || 
                    ex.Message.Contains("DELETE statement conflicted"))
                {
                    return Json(new { success = false, message = "Không thể xóa khuyến mãi này vì còn đơn hàng đang sử dụng" });
                }
                
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
