using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;
using ClosedXML.Excel;
using System.IO;

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

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            const int pageSize = 10;
            var allOrders = await _orderService.GetAllAsync();
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allOrders = allOrders.Where(o =>
                    o.OrderId.ToString().Contains(searchTerm) ||
                    (o.Customer?.Name != null && o.Customer.Name.ToLower().Contains(searchTerm)) ||
                    (o.Customer?.Phone != null && o.Customer.Phone.Contains(searchTerm)) ||
                    (o.Status != null && o.Status.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            var totalItems = allOrders.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var orders = allOrders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

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

            // Gán userId vào đơn hàng
            order.UserId = userId;

            // Gọi service để tạo đơn hàng với validation đầy đủ
            var (success, message, createdOrder) = await _orderService.CreateOrderAsync(order, orderItems);

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = message;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                order.Status = "cancelled";
                await _orderService.UpdateAsync(order);
                
                return Json(new { success = true, message = "Hủy đơn hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdateStatus(List<int> orderIds, string status)
        {
            try
            {
                if (orderIds == null || !orderIds.Any())
                {
                    return Json(new { success = false, message = "Vui lòng chọn ít nhất một đơn hàng" });
                }

                int updated = 0;
                foreach (var orderId in orderIds)
                {
                    var order = await _orderService.GetByIdAsync(orderId);
                    if (order != null)
                    {
                        order.Status = status;
                        await _orderService.UpdateAsync(order);
                        updated++;
                    }
                }

                return Json(new { success = true, message = $"Đã cập nhật {updated} đơn hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(List<int> orderIds)
        {
            try
            {
                if (orderIds == null || !orderIds.Any())
                {
                    return Json(new { success = false, message = "Vui lòng chọn ít nhất một đơn hàng" });
                }

                int deleted = 0;
                foreach (var orderId in orderIds)
                {
                    await _orderService.DeleteAsync(orderId);
                    deleted++;
                }

                return Json(new { success = true, message = $"Đã xóa {deleted} đơn hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();
                
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Đơn hàng");
                    
                    // Headers
                    worksheet.Cell(1, 1).Value = "Mã đơn hàng";
                    worksheet.Cell(1, 2).Value = "Khách hàng";
                    worksheet.Cell(1, 3).Value = "Ngày đặt";
                    worksheet.Cell(1, 4).Value = "Tổng tiền";
                    worksheet.Cell(1, 5).Value = "Giảm giá";
                    worksheet.Cell(1, 6).Value = "Trạng thái";
                    worksheet.Cell(1, 7).Value = "Sản phẩm";
                    
                    // Style headers
                    var headerRange = worksheet.Range(1, 1, 1, 7);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    
                    // Data
                    int row = 2;
                    foreach (var order in orders.OrderByDescending(o => o.OrderDate))
                    {
                        worksheet.Cell(row, 1).Value = order.OrderId;
                        worksheet.Cell(row, 2).Value = order.Customer?.Name ?? "Khách lẻ";
                        worksheet.Cell(row, 3).Value = order.OrderDate.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cell(row, 4).Value = order.TotalAmount;
                        worksheet.Cell(row, 5).Value = order.DiscountAmount;
                        worksheet.Cell(row, 6).Value = order.Status switch
                        {
                            "paid" => "Đã thanh toán",
                            "pending" => "Chờ xử lý",
                            "cancelled" => "Đã hủy",
                            "processing" => "Đang xử lý",
                            "shipped" => "Đã giao",
                            _ => order.Status
                        };
                        
                        var products = order.OrderItems != null && order.OrderItems.Any()
                            ? string.Join(", ", order.OrderItems.Select(i => $"{i.Product?.ProductName} (x{i.Quantity})"))
                            : "-";
                        worksheet.Cell(row, 7).Value = products;
                        
                        row++;
                    }
                    
                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();
                    
                    // Save to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        var fileName = $"DanhSachDonHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xuất file: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchOrders(string searchTerm)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var allOrders = await _orderService.GetAllAsync();
            var filteredOrders = allOrders.Where(o =>
                o.OrderId.ToString().Contains(searchTerm ?? "", StringComparison.OrdinalIgnoreCase) ||
                (o.Customer != null && o.Customer.Name.Contains(searchTerm ?? "", StringComparison.OrdinalIgnoreCase)) ||
                (o.OrderItems != null && o.OrderItems.Any(i => i.Product != null && i.Product.ProductName.Contains(searchTerm ?? "", StringComparison.OrdinalIgnoreCase)))
            );

            var result = filteredOrders.OrderByDescending(o => o.OrderDate).Take(12).Select(order => new
            {
                orderId = order.OrderId,
                customerName = order.Customer?.Name ?? $"Khách hàng #{order.CustomerId}",
                customerEmail = order.Customer?.Email ?? $"KH{order.CustomerId}@example.com",
                customerInitial = !string.IsNullOrEmpty(order.Customer?.Name) ? order.Customer.Name.Substring(0, 1).ToUpper() : "KH",
                products = order.OrderItems != null && order.OrderItems.Any()
                    ? order.OrderItems.First().Product?.ProductName ?? "Sản phẩm"
                    : "-",
                productCount = order.OrderItems?.Count ?? 0,
                totalAmount = order.TotalAmount,
                orderDate = order.OrderDate.ToString("dd.MM.yyyy"),
                status = order.Status,
                statusText = order.Status == "paid" ? "Đã giao" : (order.Status == "pending" ? "Đang giao" : "Chờ xử lý")
            });

            return Json(new { success = true, orders = result });
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
