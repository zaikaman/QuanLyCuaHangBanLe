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

        public async Task<IActionResult> Index()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Lấy thống kê
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Tổng doanh thu
            ViewBag.TotalRevenue = await _orderService.GetTotalRevenueAsync();
            
            // Doanh thu tháng này
            ViewBag.MonthRevenue = await _orderService.GetTotalRevenueAsync(startOfMonth, today.AddDays(1));
            
            // Tổng số đơn hàng
            ViewBag.TotalOrders = await _orderService.GetTotalOrdersAsync();
            
            // Tổng số khách hàng
            ViewBag.TotalCustomers = await _customerRepository.CountAsync();
            
            // Tổng số sản phẩm
            ViewBag.TotalProducts = await _productService.CountAsync();

            // Đơn hàng gần đây
            var allOrders = await _orderService.GetAllAsync();
            ViewBag.RecentOrders = allOrders.OrderByDescending(o => o.OrderDate).Take(5);
            
            // Thống kê đơn hàng theo trạng thái
            ViewBag.PendingOrders = allOrders.Count(o => o.Status == "pending");
            ViewBag.ProcessingOrders = allOrders.Count(o => o.Status == "processing");
            ViewBag.ShippedOrders = allOrders.Count(o => o.Status == "shipped");
            ViewBag.PaidOrders = allOrders.Count(o => o.Status == "paid");

            return View();
        }
    }
}
