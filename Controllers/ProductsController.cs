using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class ProductsController : Controller
    {
        // Temporary data - will be replaced with database
        private static List<Product> products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Coca Cola lon", CategoryId = 1, Price = 314838, Unit = "hộp", Barcode = "8900000000001" },
            new Product { ProductId = 2, ProductName = "Pepsi lon", CategoryId = 1, Price = 114807, Unit = "cái", Barcode = "8900000000002" },
            new Product { ProductId = 3, ProductName = "Trà Xanh 0 độ", CategoryId = 1, Price = 415725, Unit = "tuýp", Barcode = "8900000000003" },
            new Product { ProductId = 4, ProductName = "Sting dâu", CategoryId = 1, Price = 351670, Unit = "cái", Barcode = "8900000000004" },
            new Product { ProductId = 5, ProductName = "Red Bull", CategoryId = 1, Price = 402179, Unit = "lon", Barcode = "8900000000005" },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.ProductId = products.Count > 0 ? products.Max(p => p.ProductId) + 1 : 1;
                products.Add(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = products.FirstOrDefault(p => p.ProductId == product.ProductId);
                if (existingProduct != null)
                {
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.Price = product.Price;
                    existingProduct.Unit = product.Unit;
                    existingProduct.Barcode = product.Barcode;
                }
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                products.Remove(product);
            }
            return RedirectToAction("Index");
        }
    }
}
