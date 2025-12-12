using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SHOPIFY1.Services
{
    public class CouponService : ICouponService
    {
        private readonly ShopifyContext _context;

        public CouponService(ShopifyContext context) => _context = context;

        // =======================================================
        // Lấy danh sách
        // =======================================================
        public async Task<List<MaGiamGium>> GetAllCouponsAsync()
        {
            return await _context.MaGiamGia
                .OrderByDescending(c => c.NgayBatDau)
                .ToListAsync();
        }

        public async Task<MaGiamGium?> GetCouponByIdAsync(int id)
        {
            return await _context.MaGiamGia.FindAsync(id);
        }

        // =======================================================
        // Thêm Mã Giảm Giá (Create)
        // =======================================================
        public async Task AddCouponAsync(MaGiamGium coupon)
        {
            // Kiểm tra trùng MaCode
            if (await _context.MaGiamGia.AnyAsync(c => c.MaCode == coupon.MaCode))
                throw new Exception($"Mã code '{coupon.MaCode}' đã tồn tại. Vui lòng chọn mã khác.");

            // Kiểm tra ngày hợp lệ
            if (coupon.NgayKetThuc <= coupon.NgayBatDau)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu.");

            // Gán giá trị mặc định/ban đầu
            coupon.TrangThai = "Hoạt động";
            coupon.NgayBatDau = DateTime.Now;

            _context.MaGiamGia.Add(coupon);
            await _context.SaveChangesAsync();
        }

        // =======================================================
        // Cập nhật Mã Giảm Giá (Update)
        // =======================================================
        public async Task UpdateCouponAsync(MaGiamGium coupon)
        {
            var existing = await _context.MaGiamGia.FindAsync(coupon.MaMgg);
            if (existing == null) throw new Exception("Mã giảm giá không tồn tại.");

            // Kiểm tra trùng MaCode (trừ chính nó)
            if (await _context.MaGiamGia.AnyAsync(c => c.MaCode == coupon.MaCode && c.MaMgg != coupon.MaMgg))
                throw new Exception($"Mã code '{coupon.MaCode}' đã được sử dụng.");

            if (coupon.NgayKetThuc <= coupon.NgayBatDau)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu.");

            // Cập nhật các trường
            existing.MaCode = coupon.MaCode;
            existing.MoTa = coupon.MoTa;
            existing.PhanTramGiam = coupon.PhanTramGiam;
            existing.GiaTriToiDa = coupon.GiaTriToiDa;
            existing.NgayBatDau = coupon.NgayBatDau;
            existing.NgayKetThuc = coupon.NgayKetThuc;
            existing.SoLanSuDungToiDa = coupon.SoLanSuDungToiDa;
            existing.TrangThai = coupon.TrangThai;

            _context.MaGiamGia.Update(existing);
            await _context.SaveChangesAsync();
        }

        // =======================================================
        // Xóa Mã Giảm Giá (Delete)
        // =======================================================
        public async Task DeleteCouponAsync(int id)
        {
            var coupon = await _context.MaGiamGia.FindAsync(id);
            if (coupon == null) throw new Exception("Mã giảm giá không tồn tại.");

            // TODO: Bạn có thể thêm kiểm tra nếu mã đã được sử dụng (SuDungMaGiamGia)
            // Nếu đã được sử dụng, bạn có thể chỉ cập nhật trạng thái thành 'Hết hạn' thay vì xóa cứng.

            // Tạm thời xóa cứng (hoặc xóa mềm bằng cách thay đổi trạng thái)
            // Chúng ta sẽ dùng xóa mềm bằng cách chuyển trạng thái thành 'Vô hiệu hóa'
            coupon.TrangThai = "Vô hiệu hóa";
            _context.MaGiamGia.Update(coupon);
            await _context.SaveChangesAsync();
        }
    }
}