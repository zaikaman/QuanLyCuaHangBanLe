using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var inventories = await _inventoryRepository.GetAllAsync();
            // Load products cho mỗi inventory item
            var products = await _productService.GetAllAsync();
            
            ViewBag.Products = products;
            return View(inventories);
        }

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
