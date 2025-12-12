using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using BCrypt.Net;
using System.Security.Claims; // ✅ Thêm thư viện này
using Microsoft.AspNetCore.Authentication; // ✅ Thêm thư viện này

namespace SHOPIFY1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShopifyContext _context;

        public AccountController(ShopifyContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ✅ Đảm bảo phương thức là 'async Task<IActionResult>' để dùng await
        public async Task<IActionResult> Login(string email, string matkhau)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(matkhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return View();
            }

            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(u => u.Email == email && u.TrangThai == true);

            if (user == null || !BCrypt.Net.BCrypt.Verify(matkhau, user.MatKhau))
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                return View();
            }

            // 1. LƯU THÔNG TIN VÀO SESSION (cho RoleAuthorizeMiddleware)
            HttpContext.Session.SetInt32("UserId", user.MaTk);
            HttpContext.Session.SetString("HoTen", user.HoTen ?? "Khách hàng");
            HttpContext.Session.SetInt32("MaVaiTro", user.MaVaiTro ?? 5);

            // 2. ✅ KÝ NGƯỜI DÙNG VÀO COOKIE AUTHENTICATION
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaTk.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen ?? "Khách hàng"),
                // Thêm Claim Role để hệ thống biết vai trò người dùng (MaVaiTro)
                new Claim(ClaimTypes.Role, (user.MaVaiTro ?? 5).ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["Success"] = "Đăng nhập thành công!";

            // 3. CHUYỂN HƯỚNG
            return (user.MaVaiTro ?? 5) switch
            {
                2 => RedirectToAction("Dashboard", "Manager"), // Giữ nguyên "Manager" theo tên Controller ban đầu
                3 => RedirectToAction("Dashboard", "Employee"),
                4 => RedirectToAction("Dashboard", "Shipper"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ✅ Đảm bảo phương thức là 'async Task<IActionResult>'
        public async Task<IActionResult> Register(TaiKhoan tk)
        {
            if (string.IsNullOrWhiteSpace(tk.Email) || string.IsNullOrWhiteSpace(tk.MatKhau) || string.IsNullOrWhiteSpace(tk.HoTen))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View(tk);
            }

            if (await _context.TaiKhoans.AnyAsync(u => u.Email == tk.Email))
            {
                ViewBag.Error = "Email này đã được sử dụng.";
                return View(tk);
            }

            tk.MatKhau = BCrypt.Net.BCrypt.HashPassword(tk.MatKhau);
            tk.NgayTao = DateTime.Now;
            tk.TrangThai = true;
            tk.MaVaiTro = 5;

            _context.TaiKhoans.Add(tk);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // ✅ Cập nhật Logout thành async để SignOut khỏi Cookie
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies"); // ⬅️ Ký thoát khỏi Cookie
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }
    }
}