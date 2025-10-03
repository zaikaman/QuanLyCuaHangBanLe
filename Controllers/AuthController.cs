using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // TODO: Triển khai logic xác thực với cơ sở dữ liệu
            // Tạm thời chỉ chuyển hướng đến dashboard
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Lưu thông tin người dùng vào session
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
