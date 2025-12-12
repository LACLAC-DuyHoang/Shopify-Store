using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SHOPIFY1.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopifyContext _db;
        public OrderService(ShopifyContext db) => _db = db;

        // ✅ ĐÃ KHẮC PHỤC: Thêm tham số int? couponId = null
        public async Task<int> CheckoutAsync(int maKhachHang, string diaChi, string phuongThuc, int? couponId = null)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // get cart and items
                var cart = await _db.GioHangs.FirstOrDefaultAsync(g => g.MaKh == maKhachHang && g.TrangThai == "Đang hoạt động");
                if (cart == null) throw new Exception("Giỏ hàng rỗng");

                var items = await _db.ChiTietGioHangs.Where(i => i.MaGh == cart.MaGh).ToListAsync();
                if (!items.Any()) throw new Exception("Không có sản phẩm trong giỏ");

                // create order
                var order = new DonHang { MaKh = maKhachHang, NgayDat = DateTime.Now, TrangThai = "Chờ xác nhận", DiaChiGiao = diaChi, PhuongThucThanhToan = phuongThuc };
                _db.DonHangs.Add(order);
                await _db.SaveChangesAsync(); // get MaDH

                decimal total = 0;
                foreach (var it in items)
                {
                    var prod = await _db.SanPhams.FindAsync(it.MaSp);
                    if (prod.SoLuongTon < it.SoLuong) throw new Exception($"Sản phẩm {prod.TenSp} không đủ tồn kho");

                    var ctdh = new ChiTietDonHang { MaDh = order.MaDh, MaSp = it.MaSp, SoLuong = (int)it.SoLuong, DonGia = it.DonGia };
                    _db.ChiTietDonHangs.Add(ctdh);

                    // decrement stock
                    prod.SoLuongTon -= it.SoLuong;
                    _db.SanPhams.Update(prod);

                    total += (decimal)(it.SoLuong * it.DonGia);
                }

                // apply coupon logic here (if couponId provided)
                // TODO: Nếu bạn đã triển khai logic coupon, hãy thêm nó vào đây
                // Hiện tại, chúng ta bỏ qua nó để khắc phục lỗi biên dịch trước.


                order.TongTien = total;
                _db.DonHangs.Update(order);

                // clear cart
                _db.ChiTietGioHangs.RemoveRange(items);
                cart.TrangThai = "Đã đặt hàng";
                _db.GioHangs.Update(cart);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return order.MaDh;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // =======================================================
        // Lấy danh sách đơn hàng của Khách hàng (Đã thêm ở bước trước)
        // =======================================================
        public async Task<List<DonHang>> GetOrdersByCustomerAsync(int maKhachHang)
        {
            return await _db.DonHangs
                .Where(d => d.MaKh == maKhachHang)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        // =======================================================
        // Lấy chi tiết đơn hàng của Khách hàng (Đã thêm ở bước trước)
        // =======================================================
        public async Task<DonHang?> GetOrderDetailForCustomerAsync(int orderId, int maKhachHang)
        {
            // Chỉ lấy đơn hàng nếu nó thuộc về Khách hàng đó (để đảm bảo bảo mật)
            return await _db.DonHangs
                .Where(d => d.MaDh == orderId && d.MaKh == maKhachHang)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSpNavigation) // Chi tiết sản phẩm
                .Include(d => d.MaShipperNavigation) // Thông tin Shipper (nếu có)
                .FirstOrDefaultAsync();
        }
    }
}