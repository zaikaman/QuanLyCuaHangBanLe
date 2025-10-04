using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Supplier> _supplierRepository;

        public ProductsController(
            ProductService productService,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Supplier> supplierRepository)
        {
            _productService = productService;
            _categoryRepository = categoryRepository;
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
            var allProducts = await _productService.GetAllAsync();
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allProducts = allProducts.Where(p =>
                    (p.ProductName != null && p.ProductName.ToLower().Contains(searchTerm)) ||
                    (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)) ||
                    (p.Category?.CategoryName != null && p.Category.CategoryName.ToLower().Contains(searchTerm)) ||
                    (p.Supplier?.Name != null && p.Supplier.Name.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            var totalItems = allProducts.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            // Kiểm tra barcode trùng lặp (nếu có barcode)
            if (!string.IsNullOrWhiteSpace(product.Barcode))
            {
                var allProducts = await _productService.GetAllAsync();
                var existingProduct = allProducts.FirstOrDefault(p => 
                    p.Barcode != null && 
                    p.Barcode.Equals(product.Barcode, StringComparison.OrdinalIgnoreCase));
                
                if (existingProduct != null)
                {
                    ModelState.AddModelError("Barcode", $"Mã vạch '{product.Barcode}' đã được sử dụng cho sản phẩm '{existingProduct.ProductName}'");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.CreatedAt = DateTime.Now;
                    await _productService.AddAsync(product);
                    TempData["Success"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Bắt lỗi duplicate từ database
                    if (ex.InnerException?.Message.Contains("Duplicate entry") == true)
                    {
                        if (ex.InnerException.Message.Contains("barcode"))
                        {
                            ModelState.AddModelError("Barcode", "Mã vạch này đã tồn tại trong hệ thống");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Dữ liệu bị trùng lặp");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi thêm sản phẩm: " + ex.Message);
                    }
                }
            }
            await LoadDropdownData();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await LoadDropdownData();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            // Kiểm tra barcode trùng lặp (nếu có barcode)
            if (!string.IsNullOrWhiteSpace(product.Barcode))
            {
                var allProducts = await _productService.GetAllAsync();
                var existingProduct = allProducts.FirstOrDefault(p => 
                    p.ProductId != product.ProductId && // Không check chính nó
                    p.Barcode != null && 
                    p.Barcode.Equals(product.Barcode, StringComparison.OrdinalIgnoreCase));
                
                if (existingProduct != null)
                {
                    ModelState.AddModelError("Barcode", $"Mã vạch '{product.Barcode}' đã được sử dụng cho sản phẩm '{existingProduct.ProductName}'");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateAsync(product);
                    TempData["Success"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Bắt lỗi duplicate từ database
                    if (ex.InnerException?.Message.Contains("Duplicate entry") == true)
                    {
                        if (ex.InnerException.Message.Contains("barcode"))
                        {
                            ModelState.AddModelError("Barcode", "Mã vạch này đã tồn tại trong hệ thống");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Dữ liệu bị trùng lặp");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi cập nhật sản phẩm: " + ex.Message);
                    }
                }
            }
            await LoadDropdownData();
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction("Index");
                }

                await _productService.DeleteAsync(id);
                TempData["Success"] = "Xóa sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private async Task LoadDropdownData()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var suppliers = await _supplierRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");
        }
    }
}
