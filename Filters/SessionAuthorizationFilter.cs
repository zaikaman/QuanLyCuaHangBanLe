using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyCuaHangBanLe.Filters
{
    /// <summary>
    /// Filter để kiểm tra session authentication cho tất cả các controllers
    /// Ngoại trừ AuthController
    /// </summary>
    public class SessionAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Bỏ qua kiểm tra cho AuthController
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            if (controllerName == "Auth")
            {
                return;
            }

            // Kiểm tra session
            var username = context.HttpContext.Session.GetString("Username");
            var userId = context.HttpContext.Session.GetInt32("UserId");

            // Nếu không có session hoặc session không hợp lệ, redirect về login
            if (string.IsNullOrEmpty(username) || userId == null || userId <= 0)
            {
                Console.WriteLine($"⚠️ Unauthorized access detected - Controller: {controllerName}, Action: {context.RouteData.Values["action"]}");
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}
