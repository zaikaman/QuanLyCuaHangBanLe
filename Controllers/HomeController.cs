using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangBanLe.Data;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GlobalSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(new { success = false, message = "Vui lòng nhập ít nhất 2 ký tự" });
            }

            var searchTerm = query.ToLower().Trim();

            // Tìm kiếm sản phẩm
            var products = await _context.Products
                .Where(p => p.ProductName.ToLower().Contains(searchTerm) || 
                           (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)))
                .Select(p => new {
                    type = "product",
                    id = p.ProductId,
                    name = p.ProductName,
                    detail = $"SKU: {(p.Barcode ?? "N/A")} - {p.Price:N0}₫",
                    url = $"/Products/Details/{p.ProductId}"
                })
                .Take(5)
                .ToListAsync();

            // Tìm kiếm đơn hàng
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderId.ToString().Contains(searchTerm))
                .Select(o => new {
                    type = "order",
                    id = o.OrderId,
                    name = $"Đơn hàng #{o.OrderId.ToString().PadLeft(6, '0')}",
                    detail = o.Customer != null ? $"{o.Customer.Name} - {o.TotalAmount:N0}₫" : $"{o.TotalAmount:N0}₫",
                    url = $"/Orders/Details/{o.OrderId}"
                })
                .Take(5)
                .ToListAsync();

            // Tìm kiếm khách hàng
            var customers = await _context.Customers
                .Where(c => c.Name.ToLower().Contains(searchTerm) || 
                           (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                           (c.Phone != null && c.Phone.Contains(searchTerm)))
                .Select(c => new {
                    type = "customer",
                    id = c.CustomerId,
                    name = c.Name,
                    detail = $"{c.Email ?? "N/A"} - {c.Phone ?? "N/A"}",
                    url = $"/Customers/Details/{c.CustomerId}"
                })
                .Take(5)
                .ToListAsync();

            var results = new
            {
                success = true,
                products = products,
                orders = orders,
                customers = customers,
                hasResults = products.Any() || orders.Any() || customers.Any()
            };

            return Json(results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
