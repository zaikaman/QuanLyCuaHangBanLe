using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(User user);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}
