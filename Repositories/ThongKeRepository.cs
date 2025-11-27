// Repositories/ThongKeRepository.cs
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;

namespace SHOPIFY1.Repositories
{
    public class ThongKeRepository : IThongKeRepository
    {
        private readonly ShopifyContext _context;

        public ThongKeRepository(ShopifyContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardSummary> GetDashboardSummaryAsync()
        {
            // Sử dụng Task.WhenAll để chạy các truy vấn đồng thời
            var totalProductsTask = _context.SanPhams.CountAsync();
            var totalOrdersTask = _context.DonHangs.CountAsync();

            var totalRevenueTask = _context.DonHangs
                .Where(d => d.TrangThai == "Đã giao")
                .SumAsync(d => d.TongTien ?? 0);

            var pendingOrdersTask = _context.DonHangs
                .CountAsync(d => d.TrangThai == "Chờ xác nhận");

            await Task.WhenAll(totalProductsTask, totalOrdersTask, totalRevenueTask, pendingOrdersTask);

            return new AdminDashboardSummary
            {
                TotalProducts = totalProductsTask.Result,
                TotalOrders = totalOrdersTask.Result,
                TotalRevenue = totalRevenueTask.Result,
                PendingOrders = pendingOrdersTask.Result
            };
        }

        public async Task<Dictionary<DateTime, decimal>> GetDailyRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _context.DonHangs
                .Where(d => d.TrangThai == "Đã giao" && d.NgayDat >= startDate && d.NgayDat <= endDate)
                .GroupBy(d => d.NgayDat.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(d => d.TongTien ?? 0)
                })
                .ToListAsync();

            return result.ToDictionary(r => r.Date, r => r.Revenue);
        }

        public async Task<List<SanPham>> GetTopSellingProductsAsync(int topN)
        {
            var topSpIds = await _context.ChiTietDonHangs
               .GroupBy(ct => ct.MaSp)
               .Select(g => new
               {
                   MaSp = g.Key,
                   TotalSold = g.Sum(ct => ct.SoLuong)
               })
               .OrderByDescending(x => x.TotalSold)
               .Take(topN)
               .Select(x => x.MaSp)
               .ToListAsync();

            if (!topSpIds.Any()) return new List<SanPham>();

            return await _context.SanPhams
                .Where(p => topSpIds.Contains(p.MaSp))
                .ToListAsync();
        }
    }
}