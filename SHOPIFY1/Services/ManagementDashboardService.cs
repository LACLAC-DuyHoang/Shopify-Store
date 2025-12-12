using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SHOPIFY1.Services
{
    public class ManagementDashboardService : IManagementDashboardService
    {
        private readonly ShopifyContext _context;

        public ManagementDashboardService(ShopifyContext context) => _context = context;

        public async Task<Dictionary<string, object>> GetDashboardStatsAsync()
        {
            var stats = new Dictionary<string, object>();

            // 1. Tổng Sản phẩm đang hoạt động
            stats["TotalProducts"] = await _context.SanPhams
                .CountAsync(p => p.TrangThai == true);

            // 2. Đơn hàng mới (Chờ xác nhận)
            stats["PendingOrders"] = await _context.DonHangs
                .CountAsync(d => d.TrangThai == "Chờ xác nhận");

            // 3. Doanh thu (Hoàn tất trong tháng hiện tại)
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Tính tổng tiền của các đơn hàng có trạng thái "Hoàn tất" trong tháng hiện tại
            stats["MonthlyRevenue"] = await _context.DonHangs
                .Where(d => d.TrangThai == "Hoàn tất" && d.NgayDat >= startOfMonth)
                .SumAsync(d => d.TongTien) ?? 0m; // Sử dụng 0m nếu tổng tiền là null

            return stats;
        }
    }
}