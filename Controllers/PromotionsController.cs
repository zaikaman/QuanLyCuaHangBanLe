using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly IGenericRepository<Promotion> _promotionRepository;

        public PromotionsController(IGenericRepository<Promotion> promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var promotions = await _promotionRepository.GetAllAsync();
            return View(promotions);
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
            if (!ModelState.IsValid)
            {
                return View(promotion);
            }

            try
            {
                await _promotionRepository.AddAsync(promotion);
                TempData["SuccessMessage"] = "Thêm khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return View(promotion);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _promotionRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa khuyến mãi thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
