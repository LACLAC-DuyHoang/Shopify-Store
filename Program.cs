using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Middlewares;
using SHOPIFY1.Models;
using SHOPIFY1.Services;

namespace SHOPIFY1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ✅ THÊM DỊCH VỤ AUTHENTICATION (Xác thực) VỚI COOKIE
            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Home/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                });

            // Add services
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<SHOPIFY1.Filters.AdminRoleAttribute>();
            // Kết nối SQL Server
            builder.Services.AddDbContext<ShopifyContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 🧩 Cấu hình Session (chỉ khai báo 1 lần)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();

            // Đăng ký các dịch vụ
            builder.Services.AddScoped<IProductManagerService, ProductManagerService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            var app = builder.Build();

            // ------------------- Middleware pipeline -------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 🧩 Kích hoạt Session
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            // 🧩 Middleware custom (kiểm tra role)
            app.UseRoleAuthorize();

          

            // ------------------- Route mặc định -------------------
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
