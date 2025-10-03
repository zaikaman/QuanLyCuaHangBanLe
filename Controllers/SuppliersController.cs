using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class SuppliersController : Controller
    {
        private static List<Supplier> suppliers = new List<Supplier>
        {
            new Supplier { SupplierId = 1, Name = "Công ty TNHH ABC", Phone = "0901234567", Email = "contact@abc.com", Address = "123 Đường ABC, TP.HCM" },
            new Supplier { SupplierId = 2, Name = "Công ty CP XYZ", Phone = "0907654321", Email = "info@xyz.com", Address = "456 Đường XYZ, Hà Nội" },
            new Supplier { SupplierId = 3, Name = "Nhà phân phối 123", Phone = "0909999888", Email = "sales@123.com", Address = "789 Đường 123, Đà Nẵng" },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(suppliers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.SupplierId = suppliers.Count > 0 ? suppliers.Max(s => s.SupplierId) + 1 : 1;
                suppliers.Add(supplier);
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        public IActionResult Edit(int id)
        {
            var supplier = suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [HttpPost]
        public IActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var existingSupplier = suppliers.FirstOrDefault(s => s.SupplierId == supplier.SupplierId);
                if (existingSupplier != null)
                {
                    existingSupplier.Name = supplier.Name;
                    existingSupplier.Phone = supplier.Phone;
                    existingSupplier.Email = supplier.Email;
                    existingSupplier.Address = supplier.Address;
                }
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        public IActionResult Delete(int id)
        {
            var supplier = suppliers.FirstOrDefault(s => s.SupplierId == id);
            if (supplier != null)
            {
                suppliers.Remove(supplier);
            }
            return RedirectToAction("Index");
        }
    }
}
