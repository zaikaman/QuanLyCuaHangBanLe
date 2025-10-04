using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly IGenericRepository<Supplier> _supplierRepository;

        public SuppliersController(IGenericRepository<Supplier> supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            const int pageSize = 10;
            var allSuppliers = await _supplierRepository.GetAllAsync();
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allSuppliers = allSuppliers.Where(s =>
                    (s.Name != null && s.Name.ToLower().Contains(searchTerm)) ||
                    (s.Phone != null && s.Phone.Contains(searchTerm)) ||
                    (s.Email != null && s.Email.ToLower().Contains(searchTerm)) ||
                    (s.Address != null && s.Address.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            var totalItems = allSuppliers.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var suppliers = allSuppliers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(suppliers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
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
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                ModelState.AddModelError("Name", "Tên nhà cung cấp không được để trống");
            }

            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            try
            {
                // Kiểm tra trùng email nếu có
                if (!string.IsNullOrWhiteSpace(supplier.Email))
                {
                    var existingSuppliers = await _supplierRepository.GetAllAsync();
                    if (existingSuppliers.Any(s => s.Email != null && s.Email.Equals(supplier.Email, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng cho nhà cung cấp khác");
                        return View(supplier);
                    }
                }

                // Kiểm tra trùng số điện thoại nếu có
                if (!string.IsNullOrWhiteSpace(supplier.Phone))
                {
                    var existingSuppliers = await _supplierRepository.GetAllAsync();
                    if (existingSuppliers.Any(s => s.Phone != null && s.Phone.Equals(supplier.Phone, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError("Phone", "Số điện thoại này đã được sử dụng cho nhà cung cấp khác");
                        return View(supplier);
                    }
                }

                await _supplierRepository.AddAsync(supplier);
                TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm nhà cung cấp: " + ex.Message);
                Console.WriteLine($"❌ Lỗi thêm nhà cung cấp: {ex.Message}");
                return View(supplier);
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

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Supplier supplier)
        {
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(supplier.Name))
            {
                ModelState.AddModelError("Name", "Tên nhà cung cấp không được để trống");
            }

            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            try
            {
                var existingSupplier = await _supplierRepository.GetByIdAsync(supplier.SupplierId);
                if (existingSupplier == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy nhà cung cấp";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra trùng email (trừ chính nó)
                if (!string.IsNullOrWhiteSpace(supplier.Email))
                {
                    var allSuppliers = await _supplierRepository.GetAllAsync();
                    if (allSuppliers.Any(s => s.SupplierId != supplier.SupplierId && 
                        s.Email != null && s.Email.Equals(supplier.Email, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng cho nhà cung cấp khác");
                        return View(supplier);
                    }
                }

                // Kiểm tra trùng số điện thoại (trừ chính nó)
                if (!string.IsNullOrWhiteSpace(supplier.Phone))
                {
                    var allSuppliers = await _supplierRepository.GetAllAsync();
                    if (allSuppliers.Any(s => s.SupplierId != supplier.SupplierId && 
                        s.Phone != null && s.Phone.Equals(supplier.Phone, StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError("Phone", "Số điện thoại này đã được sử dụng cho nhà cung cấp khác");
                        return View(supplier);
                    }
                }

                existingSupplier.Name = supplier.Name.Trim();
                existingSupplier.Phone = supplier.Phone?.Trim();
                existingSupplier.Email = supplier.Email?.Trim();
                existingSupplier.Address = supplier.Address?.Trim();

                await _supplierRepository.UpdateAsync(existingSupplier);
                TempData["SuccessMessage"] = "Cập nhật nhà cung cấp thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật nhà cung cấp: " + ex.Message);
                Console.WriteLine($"❌ Lỗi cập nhật nhà cung cấp: {ex.Message}");
                return View(supplier);
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

                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhà cung cấp" });
                }

                // Kiểm tra xem nhà cung cấp có sản phẩm nào không
                if (supplier.Products != null && supplier.Products.Any())
                {
                    return Json(new { success = false, message = $"Không thể xóa nhà cung cấp '{supplier.Name}' vì còn {supplier.Products.Count} sản phẩm đang liên kết" });
                }

                await _supplierRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa nhà cung cấp thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xóa nhà cung cấp: {ex.Message}");
                
                // Kiểm tra lỗi foreign key constraint
                if (ex.InnerException?.Message.Contains("foreign key constraint") == true || 
                    ex.Message.Contains("FOREIGN KEY") || 
                    ex.Message.Contains("DELETE statement conflicted"))
                {
                    return Json(new { success = false, message = "Không thể xóa nhà cung cấp này vì còn sản phẩm đang liên kết" });
                }
                
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
