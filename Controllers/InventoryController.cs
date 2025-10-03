using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class InventoryController : Controller
    {
        private static List<Inventory> inventories = new List<Inventory>
        {
            new Inventory { InventoryId = 1, ProductId = 1, Quantity = 25, UpdatedAt = DateTime.Now },
            new Inventory { InventoryId = 2, ProductId = 2, Quantity = 169, UpdatedAt = DateTime.Now },
            new Inventory { InventoryId = 3, ProductId = 3, Quantity = 77, UpdatedAt = DateTime.Now },
            new Inventory { InventoryId = 4, ProductId = 4, Quantity = 169, UpdatedAt = DateTime.Now },
            new Inventory { InventoryId = 5, ProductId = 5, Quantity = 90, UpdatedAt = DateTime.Now },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(inventories);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var inventory = inventories.FirstOrDefault(i => i.InventoryId == id);
            if (inventory != null)
            {
                inventory.Quantity = quantity;
                inventory.UpdatedAt = DateTime.Now;
            }
            return RedirectToAction("Index");
        }
    }
}
