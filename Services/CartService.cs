using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;

namespace SHOPIFY1.Services
{
    public class CartService : ICartService
    {
        private readonly ShopifyContext _db;
        public CartService(ShopifyContext db) => _db = db;


        public async Task<GioHang> GetOrCreateCartAsync(int maKhachHang)
        {
            var cart = await _db.GioHangs.FirstOrDefaultAsync(g => g.MaKh == maKhachHang && g.TrangThai == "Đang hoạt động");
            if (cart != null) return cart;
            cart = new GioHang { MaKh = maKhachHang, NgayTao = DateTime.Now, TrangThai = "Đang hoạt động" };
            _db.GioHangs.Add(cart);
            await _db.SaveChangesAsync();
            return cart;
        }


        public async Task AddToCartAsync(int maKhachHang, int maSP, int soLuong)
        {
            var cart = await GetOrCreateCartAsync(maKhachHang);
            var product = await _db.SanPhams.FindAsync(maSP);
            if (product == null) throw new Exception("Sản phẩm không tồn tại");
            if (product.SoLuongTon < soLuong) throw new Exception("Không đủ tồn kho");


            var item = await _db.ChiTietGioHangs.FirstOrDefaultAsync(i => i.MaGh == cart.MaGh && i.MaSp == maSP);
            if (item == null)
            {
                item = new ChiTietGioHang { MaGh = cart.MaGh, MaSp = maSP, SoLuong = soLuong, DonGia = product.Gia };
                _db.ChiTietGioHangs.Add(item);
            }
            else
            {
                item.SoLuong += soLuong;
                _db.ChiTietGioHangs.Update(item);
            }
            await _db.SaveChangesAsync();
        }
    }
}
