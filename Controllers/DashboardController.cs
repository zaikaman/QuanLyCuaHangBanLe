using Microsoft.AspNetCore.Mvc;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}
