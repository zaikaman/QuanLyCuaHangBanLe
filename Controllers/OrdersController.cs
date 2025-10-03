using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class OrdersController : Controller
    {
        private static List<Order> orders = new List<Order>
        {
            new Order { OrderId = 1, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-1), Status = "paid", TotalAmount = 1192330, DiscountAmount = 0 },
            new Order { OrderId = 2, CustomerId = 2, OrderDate = DateTime.Now.AddDays(-1), Status = "paid", TotalAmount = 1731608, DiscountAmount = 0 },
            new Order { OrderId = 3, CustomerId = 3, OrderDate = DateTime.Now.AddDays(-2), Status = "pending", TotalAmount = 720782, DiscountAmount = 0 },
            new Order { OrderId = 4, CustomerId = 4, OrderDate = DateTime.Now.AddDays(-2), Status = "paid", TotalAmount = 21686, DiscountAmount = 0 },
            new Order { OrderId = 5, CustomerId = 5, OrderDate = DateTime.Now.AddDays(-3), Status = "paid", TotalAmount = 94180, DiscountAmount = 0 },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = orders.FirstOrDefault(o => o.OrderId == id);
            if (order != null)
            {
                order.Status = status;
            }
            return RedirectToAction("Index");
        }
    }
}
