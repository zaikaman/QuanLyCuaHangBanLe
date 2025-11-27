using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyCuaHangBanLe.Filters;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Supplier> _supplierRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(
            ProductService productService,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Supplier> supplierRepository,
            ICloudinaryService cloudinaryService)
        {
            _productService = productService;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _cloudinaryService = cloudinaryService;
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

        [AdminOnly]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();
            return View();
        }

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
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
                    // Upload ảnh lên Cloudinary nếu có
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        try
                        {
                            var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "products");
                            product.ImageUrl = imageUrl;
                        }
                        catch (InvalidOperationException ex)
                        {
                            ModelState.AddModelError("ImageFile", ex.Message);
                            await LoadDropdownData();
                            return View(product);
                        }
                    }

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

        [AdminOnly]
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

        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
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
                    // Upload ảnh mới nếu có
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        try
                        {
                            // Lấy sản phẩm hiện tại để lấy URL ảnh cũ
                            var currentProduct = await _productService.GetByIdAsync(id);
                            
                            // Xóa ảnh cũ trên Cloudinary nếu có
                            if (!string.IsNullOrEmpty(currentProduct?.ImageUrl))
                            {
                                var publicId = _cloudinaryService.GetPublicIdFromUrl(currentProduct.ImageUrl);
                                if (!string.IsNullOrEmpty(publicId))
                                {
                                    await _cloudinaryService.DeleteImageAsync(publicId);
                                }
                            }

                            // Upload ảnh mới
                            var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "products");
                            product.ImageUrl = imageUrl;
                        }
                        catch (InvalidOperationException ex)
                        {
                            ModelState.AddModelError("ImageFile", ex.Message);
                            await LoadDropdownData();
                            return View(product);
                        }
                    }
                    else
                    {
                        // Giữ nguyên ảnh cũ nếu không upload ảnh mới
                        var currentProduct = await _productService.GetByIdAsync(id);
                        product.ImageUrl = currentProduct?.ImageUrl;
                    }

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

        [AdminOnly]
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

        [AdminOnly]
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

                // Kiểm tra xem sản phẩm có đang được sử dụng trong đơn hàng không
                var (canDelete, message) = await _productService.CanDeleteProductAsync(id);
                if (!canDelete)
                {
                    TempData["Error"] = message;
                    return RedirectToAction("Index");
                }

                // Xóa ảnh trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var publicId = _cloudinaryService.GetPublicIdFromUrl(product.ImageUrl);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(publicId);
                    }
                }

                await _productService.DeleteAsync(id);
                TempData["Success"] = "Xóa sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xóa sản phẩm: {ex.Message}");
                
                // Kiểm tra lỗi foreign key constraint
                if (ex.InnerException?.Message.Contains("foreign key constraint") == true || 
                    ex.Message.Contains("FOREIGN KEY") || 
                    ex.Message.Contains("DELETE statement conflicted"))
                {
                    TempData["Error"] = "Không thể xóa sản phẩm này vì đã có đơn hàng sử dụng";
                }
                else
                {
                    TempData["Error"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
                }
                
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
