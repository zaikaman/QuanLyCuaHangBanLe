using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using DotNetEnv;
using QuanLyCuaHangBanLe.Data;
using QuanLyCuaHangBanLe.Services;
using QuanLyCuaHangBanLe.Models;
// using QuanLyCuaHangBanLe.Filters; // TODO: Uncomment sau khi build lần đầu

namespace QuanLyCuaHangBanLe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load .env file nếu tồn tại (chỉ cho local development)
            if (File.Exists(".env"))
            {
                Env.Load();
            }

            var builder = WebApplication.CreateBuilder(args);

            // Kiểm tra môi trường Heroku
            var isHeroku = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO"));

            // Cấu hình DbContext với MySQL
            // Ưu tiên: JAWSDB_URL (Heroku) -> DATABASE_URL (.env) -> appsettings.json
            string? connectionString;
            var jawsDbUrl = Environment.GetEnvironmentVariable("JAWSDB_URL");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            
            if (!string.IsNullOrEmpty(jawsDbUrl))
            {
                // Heroku với JawsDB
                connectionString = ConvertMySqlUrlToConnectionString(jawsDbUrl);
            }
            else if (!string.IsNullOrEmpty(databaseUrl))
            {
                // Local với .env file (DATABASE_URL)
                connectionString = ConvertMySqlUrlToConnectionString(databaseUrl);
            }
            else
            {
                // Fallback: appsettings.json
                connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            }
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Cấu hình Forwarded Headers cho Heroku (proxy)
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                if (isHeroku)
                {
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                }
            });

            // Cấu hình HTTPS redirection cho Heroku
            if (isHeroku)
            {
                builder.Services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                    options.HttpsPort = 443;
                });
            }

            // Đăng ký các services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
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
            builder.Services.AddControllersWithViews(options =>
            {
                // TODO: Uncomment sau khi build lần đầu
                // Đăng ký SessionAuthorizationFilter cho tất cả controllers
                // options.Filters.Add<SessionAuthorizationFilter>();
            });
            
            // Thêm hỗ trợ session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Forwarded headers phải đứng đầu pipeline
            app.UseForwardedHeaders();

            // Cấu hình HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // Giá trị HSTS mặc định là 30 ngày. Bạn có thể muốn thay đổi điều này cho các kịch bản production, xem https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
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

        /// <summary>
        /// Chuyển đổi JAWSDB_URL (mysql://user:pass@host:port/db) sang connection string của Pomelo
        /// </summary>
        private static string? ConvertMySqlUrlToConnectionString(string? mysqlUrl)
        {
            if (string.IsNullOrEmpty(mysqlUrl))
                return null;

            var uri = new Uri(mysqlUrl);
            var userInfo = uri.UserInfo.Split(':');
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 3306;
            var database = uri.AbsolutePath.TrimStart('/');
            var user = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";

            return $"Server={host};Port={port};Database={database};User={user};Password={password};";
        }
    }
}
