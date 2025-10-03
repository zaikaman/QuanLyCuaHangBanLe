using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangBanLe.Data;
using QuanLyCuaHangBanLe.Services;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DbContext với MySQL
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Đăng ký các services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            builder.Services.AddScoped<IGenericRepository<Customer>, GenericRepository<Customer>>();
            builder.Services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
            builder.Services.AddScoped<IGenericRepository<Supplier>, GenericRepository<Supplier>>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<IGenericRepository<Inventory>, GenericRepository<Inventory>>();
            builder.Services.AddScoped<IGenericRepository<Promotion>, GenericRepository<Promotion>>();
            builder.Services.AddScoped<IGenericRepository<Payment>, GenericRepository<Payment>>();

            // Thêm các dịch vụ vào container
            builder.Services.AddControllersWithViews();
            
            // Thêm hỗ trợ session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Cấu hình HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // Giá trị HSTS mặc định là 30 ngày. Bạn có thể muốn thay đổi điều này cho các kịch bản production, xem https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseRouting();
            
            // Kích hoạt session
            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
