using Microsoft.AspNetCore.Mvc;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Check if user is logged in
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}
