using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Validation chi tiết
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add("Tên đăng nhập không được để trống");
            }
            else if (username.Length < 3)
            {
                errors.Add("Tên đăng nhập phải có ít nhất 3 ký tự");
            }
            else if (username.Length > 50)
            {
                errors.Add("Tên đăng nhập không được vượt quá 50 ký tự");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Mật khẩu không được để trống");
            }
            else if (password.Length < 6)
            {
                errors.Add("Mật khẩu phải có ít nhất 6 ký tự");
            }

            if (errors.Any())
            {
                ViewBag.Error = string.Join(", ", errors);
                return View();
            }

            try
            {
                var user = await _authService.AuthenticateAsync(username.Trim(), password);
                
                if (user != null)
                {
                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("FullName", user.FullName ?? "");
                    HttpContext.Session.SetString("Role", user.Role);
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    
                    Console.WriteLine($"✅ Đăng nhập thành công: {user.Username} (ID: {user.UserId})");
                    return RedirectToAction("Index", "Dashboard");
                }

                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                Console.WriteLine($"❌ Đăng nhập thất bại cho username: {username}");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi đăng nhập: {ex.Message}");
                ViewBag.Error = "Có lỗi xảy ra trong quá trình đăng nhập. Vui lòng thử lại sau.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            // Validation
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                errors.Add("Mật khẩu cũ không được để trống");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errors.Add("Mật khẩu mới không được để trống");
            }
            else if (newPassword.Length < 6)
            {
                errors.Add("Mật khẩu mới phải có ít nhất 6 ký tự");
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                errors.Add("Xác nhận mật khẩu không được để trống");
            }

            if (!string.IsNullOrWhiteSpace(newPassword) && !string.IsNullOrWhiteSpace(confirmPassword))
            {
                if (newPassword != confirmPassword)
                {
                    errors.Add("Mật khẩu mới và xác nhận mật khẩu không khớp");
                }
            }

            if (errors.Any())
            {
                TempData["Error"] = string.Join(", ", errors);
                return View();
            }

            try
            {
                var result = await _authService.ChangePasswordAsync(userId.Value, oldPassword, newPassword);

                if (result)
                {
                    TempData["Success"] = "Đổi mật khẩu thành công!";
                    Console.WriteLine($"✅ Đổi mật khẩu thành công cho user ID: {userId}");
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    TempData["Error"] = "Mật khẩu cũ không đúng";
                    Console.WriteLine($"❌ Đổi mật khẩu thất bại: Mật khẩu cũ không đúng");
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi đổi mật khẩu: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra trong quá trình đổi mật khẩu. Vui lòng thử lại sau.";
                return View();
            }
        }
    }
}
