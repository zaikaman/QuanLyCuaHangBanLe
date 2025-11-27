using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Filters;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    [AdminOnly]
    public class UsersController : Controller
    {
        private readonly IGenericRepository<User> _userRepository;

        public UsersController(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            const int pageSize = 10;
            var allUsers = await _userRepository.GetAllAsync();
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allUsers = allUsers.Where(u =>
                    (u.Username != null && u.Username.ToLower().Contains(searchTerm)) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(searchTerm)) ||
                    (u.Role != null && u.Role.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            var totalItems = allUsers.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = allUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(users);
        }

        public IActionResult Create()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (user == null)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập không được để trống");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Mật khẩu không được để trống");
            }

            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                ModelState.AddModelError("FullName", "Họ tên không được để trống");
            }

            // Kiểm tra username trùng
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                var existingUsers = await _userRepository.GetAllAsync();
                if (existingUsers.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.CreatedAt = DateTime.Now;
                    user.Username = user.Username.Trim();
                    user.FullName = user.FullName.Trim();
                    
                    await _userRepository.AddAsync(user);
                    TempData["Success"] = "Thêm người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi thêm người dùng: {ex.Message}");
                    
                    // Kiểm tra lỗi duplicate key
                    if (ex.InnerException?.Message.Contains("Duplicate entry") == true || 
                        ex.Message.Contains("duplicate key"))
                    {
                        ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi thêm người dùng: " + ex.Message);
                    }
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != user.UserId)
            {
                TempData["Error"] = "Dữ liệu không khớp";
                return RedirectToAction(nameof(Index));
            }

            if (user == null)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập không được để trống");
            }

            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                ModelState.AddModelError("FullName", "Họ tên không được để trống");
            }

            // Kiểm tra username trùng (trừ chính nó)
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                var allUsers = await _userRepository.GetAllAsync();
                if (allUsers.Any(u => u.UserId != user.UserId && 
                    u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Username = user.Username.Trim();
                    user.FullName = user.FullName.Trim();
                    
                    await _userRepository.UpdateAsync(user);
                    TempData["Success"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi cập nhật người dùng: {ex.Message}");
                    
                    // Kiểm tra lỗi duplicate key
                    if (ex.InnerException?.Message.Contains("Duplicate entry") == true || 
                        ex.Message.Contains("duplicate key"))
                    {
                        ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi cập nhật người dùng: " + ex.Message);
                    }
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "ID không hợp lệ";
                    return RedirectToAction("Index");
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy người dùng";
                    return RedirectToAction("Index");
                }

                // Không cho xóa chính mình
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (currentUserId == id)
                {
                    TempData["Error"] = "Không thể xóa tài khoản của chính bạn";
                    return RedirectToAction("Index");
                }

                // Kiểm tra xem user có đơn hàng nào không
                if (user.Orders != null && user.Orders.Any())
                {
                    TempData["Error"] = $"Không thể xóa người dùng '{user.FullName}' vì còn {user.Orders.Count} đơn hàng liên quan";
                    return RedirectToAction("Index");
                }

                await _userRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa người dùng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xóa người dùng: {ex.Message}");
                
                // Kiểm tra lỗi foreign key constraint
                if (ex.InnerException?.Message.Contains("foreign key constraint") == true || 
                    ex.Message.Contains("FOREIGN KEY") || 
                    ex.Message.Contains("DELETE statement conflicted"))
                {
                    TempData["Error"] = "Không thể xóa người dùng này vì còn đơn hàng liên quan";
                }
                else
                {
                    TempData["Error"] = "Lỗi khi xóa người dùng: " + ex.Message;
                }
                
                return RedirectToAction("Index");
            }
        }
    }
}
