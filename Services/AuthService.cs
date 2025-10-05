using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangBanLe.Data;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            // Tìm user theo username và password (trong thực tế nên hash password)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            
            return user;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username);
                
                if (existingUser != null)
                {
                    return false;
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                {
                    return false;
                }

                // Kiểm tra mật khẩu cũ có đúng không
                if (user.Password != oldPassword)
                {
                    return false;
                }

                // Cập nhật mật khẩu mới
                user.Password = newPassword;
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
