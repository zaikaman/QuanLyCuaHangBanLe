using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class DashboardController : Controller
    {
        private readonly OrderService _orderService;
        private readonly IGenericRepository<Models.Customer> _customerRepository;
        private readonly ProductService _productService;

        public DashboardController(
            OrderService orderService,
            IGenericRepository<Models.Customer> customerRepository,
            ProductService productService)
        {
            _orderService = orderService;
            _customerRepository = customerRepository;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string dateRange = "week", DateTime? startDate = null, DateTime? endDate = null)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Kiểm tra thông báo lỗi từ session (ví dụ: khi bị từ chối quyền truy cập)
            var errorMessage = HttpContext.Session.GetString("ErrorMessage");
            if (!string.IsNullOrEmpty(errorMessage))
            {
                TempData["Error"] = errorMessage;
                HttpContext.Session.Remove("ErrorMessage");
            }

            // Lấy thống kê
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Xác định khoảng thời gian
            DateTime filterStart, filterEnd;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                filterStart = startDate.Value;
                filterEnd = endDate.Value;
            }
            else
            {
                switch (dateRange.ToLower())
                {
                    case "today":
                        filterStart = today;
                        filterEnd = today.AddDays(1);
                        break;
                    case "week":
                        filterStart = today.AddDays(-(int)today.DayOfWeek);
                        filterEnd = today.AddDays(1);
                        break;
                    case "month":
                        filterStart = startOfMonth;
                        filterEnd = today.AddDays(1);
                        break;
                    default:
                        filterStart = today.AddDays(-(int)today.DayOfWeek);
                        filterEnd = today.AddDays(1);
                        break;
                }
            }

            // Tổng doanh thu
            ViewBag.TotalRevenue = await _orderService.GetTotalRevenueAsync();
            
            // Doanh thu tháng này
            ViewBag.MonthRevenue = await _orderService.GetTotalRevenueAsync(startOfMonth, today.AddDays(1));
            
            // Doanh thu tháng trước (để so sánh)
            var lastMonthStart = startOfMonth.AddMonths(-1);
            var lastMonthEnd = startOfMonth;
            var lastMonthRevenue = await _orderService.GetTotalRevenueAsync(lastMonthStart, lastMonthEnd);
            ViewBag.LastMonthRevenue = lastMonthRevenue;
            
            // Doanh thu theo khoảng thời gian được chọn
            ViewBag.FilteredRevenue = await _orderService.GetTotalRevenueAsync(filterStart, filterEnd);
            
            // Tổng số đơn hàng
            ViewBag.TotalOrders = await _orderService.GetTotalOrdersAsync();
            
            // Đơn hàng tháng này và tháng trước
            var ordersThisMonth = await _orderService.GetTotalOrdersAsync(startOfMonth, today.AddDays(1));
            var ordersLastMonth = await _orderService.GetTotalOrdersAsync(lastMonthStart, lastMonthEnd);
            ViewBag.OrdersThisMonth = ordersThisMonth;
            ViewBag.OrdersLastMonth = ordersLastMonth;
            
            // Tổng số khách hàng
            ViewBag.TotalCustomers = await _customerRepository.CountAsync();
            
            // Tổng số sản phẩm
            ViewBag.TotalProducts = await _productService.CountAsync();

            // Doanh thu theo tháng (cho biểu đồ)
            var monthlyRevenue = await _orderService.GetMonthlyRevenueAsync(6);
            ViewBag.MonthlyRevenueLabels = monthlyRevenue.Keys.ToList();
            ViewBag.MonthlyRevenueData = monthlyRevenue.Values.ToList();

            // Doanh thu theo danh mục (cho biểu đồ donut)
            var categoryRevenue = await _orderService.GetRevenueByCategoryAsync();
            ViewBag.CategoryLabels = categoryRevenue.Keys.ToList();
            ViewBag.CategoryData = categoryRevenue.Values.ToList();
            var totalCategoryRevenue = categoryRevenue.Values.Sum();
            ViewBag.CategoryPercentages = categoryRevenue.ToDictionary(
                x => x.Key,
                x => totalCategoryRevenue > 0 ? Math.Round((x.Value / totalCategoryRevenue) * 100, 1) : 0
            );

            // Đơn hàng gần đây (theo bộ lọc)
            var allOrders = await _orderService.GetAllAsync();
            var filteredOrders = allOrders.Where(o => o.OrderDate >= filterStart && o.OrderDate < filterEnd).ToList();
            
            ViewBag.RecentOrders = filteredOrders.OrderByDescending(o => o.OrderDate).Take(5);
            
            // Thống kê đơn hàng theo trạng thái (theo bộ lọc)
            ViewBag.PendingOrders = filteredOrders.Count(o => o.Status == "pending");
            ViewBag.PaidOrders = filteredOrders.Count(o => o.Status == "paid");
            ViewBag.CanceledOrders = filteredOrders.Count(o => o.Status == "canceled");
            
            ViewBag.DateRange = dateRange;
            ViewBag.StartDate = filterStart;
            ViewBag.EndDate = filterEnd;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var allOrders = await _orderService.GetAllAsync();
            var filteredOrders = string.IsNullOrEmpty(status) 
                ? allOrders 
                : allOrders.Where(o => o.Status == status);

            var result = filteredOrders.OrderByDescending(o => o.OrderDate).Take(5).Select(order => new
            {
                orderId = order.OrderId,
                customerName = order.Customer?.Name ?? "Khách lẻ",
                products = order.OrderItems != null && order.OrderItems.Any()
                    ? string.Join(", ", order.OrderItems.Take(2).Select(i => i.Product?.ProductName ?? ""))
                    : "-",
                orderDate = order.OrderDate.ToString("dd/MM/yyyy"),
                totalAmount = order.TotalAmount,
                status = order.Status,
                statusText = order.Status switch
                {
                    "paid" => "Đã thanh toán",
                    "pending" => "Chờ xử lý",
                    "canceled" => "Đã hủy",
                    _ => "Chờ xử lý"
                }
            });

            return Json(new { success = true, orders = result });
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

            var result = filteredOrders.OrderByDescending(o => o.OrderDate).Take(5).Select(order => new
            {
                orderId = order.OrderId,
                customerName = order.Customer?.Name ?? "Khách lẻ",
                products = order.OrderItems != null && order.OrderItems.Any()
                    ? string.Join(", ", order.OrderItems.Take(2).Select(i => i.Product?.ProductName ?? ""))
                    : "-",
                orderDate = order.OrderDate.ToString("dd/MM/yyyy"),
                totalAmount = order.TotalAmount,
                status = order.Status,
                statusText = order.Status switch
                {
                    "paid" => "Đã thanh toán",
                    "pending" => "Chờ xử lý",
                    "canceled" => "Đã hủy",
                    _ => "Chờ xử lý"
                }
            });

            return Json(new { success = true, orders = result });
        }
    }
}
