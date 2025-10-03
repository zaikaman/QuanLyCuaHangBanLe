using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly ProductService _productService;

        public OrdersController(
            OrderService orderService,
            IGenericRepository<Customer> customerRepository,
            ProductService productService)
        {
            _orderService = orderService;
            _customerRepository = customerRepository;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<IActionResult> Create()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            await LoadDropdownData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProductInfo(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            return Json(new
            {
                success = true,
                productId = product.ProductId,
                productName = product.ProductName,
                price = product.Price,
                unit = product.Unit
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, List<OrderItem> orderItems)
        {
            try
            {
                var username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Lấy userId từ session
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (!int.TryParse(userIdStr, out int userId))
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng";
                    return RedirectToAction("Index");
                }

                order.UserId = userId;
                order.OrderDate = DateTime.Now;
                order.DiscountAmount = order.DiscountAmount > 0 ? order.DiscountAmount : 0;
                
                // Tính tổng tiền từ order items
                decimal totalAmount = 0;
                foreach (var item in orderItems.Where(i => i.ProductId > 0))
                {
                    var product = await _productService.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        item.Price = product.Price;
                        item.Subtotal = item.Quantity * item.Price;
                        totalAmount += item.Subtotal;
                    }
                }

                order.TotalAmount = totalAmount;
                order.OrderItems = orderItems.Where(i => i.ProductId > 0).ToList();

                await _orderService.AddAsync(order);
                TempData["Success"] = "Tạo đơn hàng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tạo đơn hàng: " + ex.Message;
                await LoadDropdownData();
                return View(order);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                order.Status = status;
                await _orderService.UpdateAsync(order);
                
                return Json(new { success = true, message = "Cập nhật trạng thái thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        private async Task LoadDropdownData()
        {
            var customers = await _customerRepository.GetAllAsync();
            var products = await _productService.GetAllAsync();

            ViewBag.Customers = new SelectList(customers, "CustomerId", "Name");
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName");
        }
    }
}
