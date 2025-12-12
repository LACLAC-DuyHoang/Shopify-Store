using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SHOPIFY1.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ShopifyContext _context;

        public EmployeeService(ShopifyContext context) => _context = context;

        // Vai trò Shipper là 4
        private const int ShipperRole = 4;

        // --------------------------------------------------------------------
        // Lấy đơn hàng
        // --------------------------------------------------------------------
        public async Task<List<DonHang>> GetPendingOrdersAsync()
        {
            // Lấy các đơn hàng đang ở trạng thái chờ xác nhận hoặc đang xử lý
            return await _context.DonHangs
                .Where(d => d.TrangThai == "Chờ xác nhận" || d.TrangThai == "Đang xử lý")
                .Include(d => d.MaKhNavigation) // Lấy thông tin khách hàng
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<DonHang?> GetOrderDetailsAsync(int orderId)
        {
            // Lấy chi tiết đơn hàng, bao gồm cả chi tiết sản phẩm và shipper (nếu có)
            return await _context.DonHangs
                .Include(d => d.MaKhNavigation) // Khách hàng
                .Include(d => d.MaShipperNavigation) // Shipper được gán
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSpNavigation) // Chi tiết sản phẩm
                .FirstOrDefaultAsync(d => d.MaDh == orderId);
        }

        // --------------------------------------------------------------------
        // Quản lý trạng thái và Shipper
        // --------------------------------------------------------------------
        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.DonHangs.FindAsync(orderId);
            if (order == null) throw new Exception("Đơn hàng không tồn tại.");

            if (newStatus == "Đã hủy")
            {
                // Nếu đơn hàng bị hủy, cần hoàn lại tồn kho sản phẩm (nếu đã trừ ở Checkout)
                // Tuy nhiên, logic này phức tạp, ta tạm thời chỉ hủy đơn hàng.
                order.TrangThai = "Đã hủy";
                order.MaShipper = null;
            }
            else if (newStatus == "Xác nhận")
            {
                order.TrangThai = "Đang xử lý"; // Trạng thái sau khi nhân viên xác nhận
            }
            else
            {
                throw new ArgumentException("Trạng thái cập nhật không hợp lệ.");
            }

            _context.DonHangs.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TaiKhoan>> GetAvailableShippersAsync()
        {
            // Lấy danh sách tài khoản có MaVaiTro = 4 (Shipper)
            return await _context.TaiKhoans
                .Where(tk => tk.MaVaiTro == ShipperRole && tk.TrangThai == true)
                .OrderBy(tk => tk.HoTen)
                .ToListAsync();
        }

        public async Task AssignShipperAsync(int orderId, int shipperId)
        {
            var order = await _context.DonHangs.FindAsync(orderId);
            if (order == null) throw new Exception("Đơn hàng không tồn tại.");

            var shipper = await _context.TaiKhoans.FindAsync(shipperId);
            if (shipper == null || shipper.MaVaiTro != ShipperRole)
                throw new Exception("Shipper không hợp lệ.");

            // 1. Gán Shipper và cập nhật trạng thái
            order.MaShipper = shipperId;
            order.TrangThai = "Đang giao hàng";
            _context.DonHangs.Update(order);

            // 2. Tạo lịch sử giao hàng
            _context.LichSuGiaoHangs.Add(new LichSuGiaoHang
            {
                MaDh = orderId,
                MaShipper = shipperId,
                TrangThaiGiao = "Đang giao hàng",
                NgayCapNhat = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}