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

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            const int pageSize = 10;
            var allCategories = await _categoryRepository.GetAllAsync();
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allCategories = allCategories.Where(c =>
                    c.CategoryName != null && c.CategoryName.ToLower().Contains(searchTerm)
                ).ToList();
            }
            
            var totalItems = allCategories.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var categories = allCategories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(categories);
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
        public async Task<IActionResult> Create(Category category)
        {
            // Validate dữ liệu trước
            if (category == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục không được để trống");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                // Kiểm tra trùng tên danh mục
                var existingCategories = await _categoryRepository.GetAllAsync();
                if (existingCategories.Any(c => c.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại");
                    return View(category);
                }

                await _categoryRepository.AddAsync(category);
                TempData["SuccessMessage"] = "Thêm loại sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm danh mục: " + ex.Message);
                Console.WriteLine($"❌ Lỗi thêm danh mục: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                }
                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax(string categoryName)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    return Json(new { success = false, message = "Tên danh mục không được để trống" });
                }

                if (categoryName.Length < 2 || categoryName.Length > 100)
                {
                    return Json(new { success = false, message = "Tên danh mục phải có từ 2 đến 100 ký tự" });
                }

                // Kiểm tra trùng tên
                var existingCategories = await _categoryRepository.GetAllAsync();
                if (existingCategories.Any(c => c.CategoryName.Equals(categoryName.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    return Json(new { success = false, message = "Tên danh mục này đã tồn tại" });
                }

                var category = new Category { CategoryName = categoryName.Trim() };
                await _categoryRepository.AddAsync(category);
                return Json(new { success = true, message = "Thêm loại sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi CreateAjax danh mục: {ex.Message}");
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
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

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            // Validate dữ liệu
            if (category == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục không được để trống");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            try
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(category.CategoryId);
                if (existingCategory == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy loại sản phẩm";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra trùng tên (trừ chính nó)
                var allCategories = await _categoryRepository.GetAllAsync();
                if (allCategories.Any(c => c.CategoryId != category.CategoryId && 
                    c.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại");
                    return View(category);
                }

                existingCategory.CategoryName = category.CategoryName.Trim();
                await _categoryRepository.UpdateAsync(existingCategory);
                TempData["SuccessMessage"] = "Cập nhật loại sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật danh mục: " + ex.Message);
                Console.WriteLine($"❌ Lỗi cập nhật danh mục: {ex.Message}");
                return View(category);
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

                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy loại sản phẩm" });
                }

                // Kiểm tra xem danh mục có sản phẩm nào không
                if (category.Products != null && category.Products.Any())
                {
                    return Json(new { success = false, message = $"Không thể xóa danh mục '{category.CategoryName}' vì còn {category.Products.Count} sản phẩm đang sử dụng" });
                }

                await _categoryRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa loại sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xóa danh mục: {ex.Message}");
                
                // Kiểm tra lỗi foreign key constraint
                if (ex.InnerException?.Message.Contains("foreign key constraint") == true || 
                    ex.Message.Contains("FOREIGN KEY") || 
                    ex.Message.Contains("DELETE statement conflicted"))
                {
                    return Json(new { success = false, message = "Không thể xóa danh mục này vì còn sản phẩm đang sử dụng" });
                }
                
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
