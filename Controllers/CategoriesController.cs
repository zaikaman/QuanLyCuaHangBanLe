using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IGenericRepository<Category> _categoryRepository;

        public CategoriesController(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string categoryName)
        {
            try
            {
                var category = new Category { CategoryName = categoryName };
                await _categoryRepository.AddAsync(category);
                return Json(new { success = true, message = "Thêm loại sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int categoryId, string categoryName)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy loại sản phẩm" });
                }

                category.CategoryName = categoryName;
                await _categoryRepository.UpdateAsync(category);
                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy loại sản phẩm" });
                }

                await _categoryRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa loại sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
