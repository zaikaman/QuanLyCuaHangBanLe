using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Filters;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IGenericRepository<Inventory> _inventoryRepository;
        private readonly ProductService _productService;

        public InventoryController(
            IGenericRepository<Inventory> inventoryRepository,
            ProductService productService)
        {
            _inventoryRepository = inventoryRepository;
            _productService = productService;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            const int pageSize = 10;
            var allInventories = await _inventoryRepository.GetAllAsync();
            
            // Load products để có thể search
            var products = await _productService.GetAllAsync();
            var productDict = products.ToDictionary(p => p.ProductId, p => p);
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allInventories = allInventories.Where(i =>
                    i.ProductId.ToString().Contains(searchTerm) ||
                    (productDict.ContainsKey(i.ProductId) && 
                     productDict[i.ProductId].ProductName != null && 
                     productDict[i.ProductId].ProductName!.ToLower().Contains(searchTerm)) ||
                    (productDict.ContainsKey(i.ProductId) && 
                     productDict[i.ProductId].Barcode != null && 
                     productDict[i.ProductId].Barcode!.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            var totalItems = allInventories.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var inventories = allInventories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            ViewBag.Products = products;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;
            
            // Truyền thông tin role để view biết ẩn/hiện nút cập nhật
            ViewBag.UserRole = HttpContext.Session.GetString("Role") ?? "staff";

            return View(inventories);
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int inventoryId, int quantity)
        {
            try
            {
                var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
                if (inventory == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin tồn kho" });
                }

                inventory.Quantity = quantity;
                inventory.UpdatedAt = DateTime.Now;
                await _inventoryRepository.UpdateAsync(inventory);
                
                return Json(new { success = true, message = "Cập nhật số lượng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
