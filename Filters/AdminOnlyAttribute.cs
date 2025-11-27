using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyCuaHangBanLe.Filters
{
    /// <summary>
    /// Attribute để giới hạn truy cập chỉ cho Admin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
            {
                Console.WriteLine($"⛔ Truy cập bị từ chối - Yêu cầu quyền Admin. Role hiện tại: {role}");
                
                // Redirect về Dashboard với thông báo lỗi
                context.HttpContext.Session.SetString("ErrorMessage", "Bạn không có quyền truy cập chức năng này. Chỉ Admin mới được phép.");
                context.Result = new RedirectToActionResult("Index", "Dashboard", null);
            }
        }
    }
}
