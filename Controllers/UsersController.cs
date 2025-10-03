using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class UsersController : Controller
    {
        private static List<User> users = new List<User>
        {
            new User { UserId = 1, Username = "admin", Password = "123456", FullName = "Quản trị viên", Role = "admin", CreatedAt = DateTime.Now.AddMonths(-6) },
            new User { UserId = 2, Username = "staff01", Password = "123456", FullName = "Nguyễn Văn A", Role = "staff", CreatedAt = DateTime.Now.AddMonths(-3) },
            new User { UserId = 3, Username = "staff02", Password = "123456", FullName = "Lê Thị B", Role = "staff", CreatedAt = DateTime.Now.AddMonths(-2) },
            new User { UserId = 4, Username = "staff03", Password = "123456", FullName = "Trần Văn C", Role = "staff", CreatedAt = DateTime.Now.AddMonths(-1) },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

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
        public IActionResult Create(User user)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                user.UserId = users.Any() ? users.Max(u => u.UserId) + 1 : 1;
                user.CreatedAt = DateTime.Now;
                users.Add(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public IActionResult Edit(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = users.FirstOrDefault(u => u.UserId == id);
            
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(int id, User user)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingUser = users.FirstOrDefault(u => u.UserId == id);
                if (existingUser != null)
                {
                    existingUser.Username = user.Username;
                    existingUser.FullName = user.FullName;
                    existingUser.Role = user.Role;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                users.Remove(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
