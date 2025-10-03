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
            // TODO: Implement authentication logic with database
            // For now, just redirect to dashboard
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Store user info in session
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
