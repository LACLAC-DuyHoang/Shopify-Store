using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHOPIFY1.Extensions;
using SHOPIFY1.Models;

namespace SHOPIFY1.Controllers
{
    [Authorize] // BẮT BUỘC ĐĂNG NHẬP
    public class CheckoutController : Controller
    {
        private readonly ShopifyContext _context;

        public CheckoutController(ShopifyContext context) => _context = context;

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            ViewBag.MaGiamGias = _context.MaGiamGia
                .Where(m => m.TrangThai == "Hoạt động" &&
                            m.NgayBatDau <= DateTime.Now &&
                            m.NgayKetThuc >= DateTime.Now)
                .ToList();

            return View(cart);
        }

        [HttpPost]
        public IActionResult PlaceOrder(string diaChiGiao, string phuongThucThanhToan, string maCode = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return Unauthorized();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (!cart?.Any() ?? true) return BadRequest("Giỏ hàng trống");

            // Tính tổng tiền + giảm giá
            decimal tongTien = cart.Sum(c => c.ThanhTien);
            decimal giamGia = 0;

            if (!string.IsNullOrEmpty(maCode))
            {
                var maGiam = _context.MaGiamGia.FirstOrDefault(m => m.MaCode == maCode && m.TrangThai == "Hoạt động");
                if (maGiam != null && maGiam.NgayBatDau <= DateTime.Now && maGiam.NgayKetThuc >= DateTime.Now)
                {
                    giamGia = Math.Min((decimal)(tongTien * maGiam.PhanTramGiam / 100), maGiam.GiaTriToiDa ?? decimal.MaxValue);
                    tongTien -= giamGia;
                }
            }

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaKh = userId,
                NgayDat = DateTime.Now,
                TongTien = tongTien,
                TrangThai = "Chờ xác nhận",
                DiaChiGiao = diaChiGiao,
                PhuongThucThanhToan = phuongThucThanhToan
            };
            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            // Chi tiết + cập nhật tồn kho
            foreach (var item in cart)
            {
                var sp = _context.SanPhams.Find(item.MaSp);
                if (sp.SoLuongTon < item.SoLuong) throw new Exception("Hết hàng!");

                _context.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaSp = item.MaSp,
                    SoLuong = item.SoLuong,
                    DonGia = item.Gia
                });

                sp.SoLuongTon -= item.SoLuong; // Cập nhật tồn kho
            }

            // Lưu mã giảm giá đã dùng
            if (giamGia > 0)
            {
                _context.SuDungMaGiamGia.Add(new SuDungMaGiamGium
                {
                    MaMgg = _context.MaGiamGia.First(m => m.MaCode == maCode).MaMgg,
                    MaKh = (int)userId,
                    MaDh = donHang.MaDh,
                    SoTienGiam = giamGia
                });
            }

            _context.SaveChanges();
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = $"Đặt hàng thành công! Mã đơn: {donHang.MaDh}";
            return RedirectToAction("Success");
        }

        public IActionResult Success() => View();
    }
}
