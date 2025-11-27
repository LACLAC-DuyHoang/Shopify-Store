// Repositories/IThongKeRepository.cs
using SHOPIFY1.Models;
using System.Collections.Generic;

namespace SHOPIFY1.Repositories
{
    // Model cho Dashboard Summary
    public class AdminDashboardSummary
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
    }

    public interface IThongKeRepository
    {
        Task<AdminDashboardSummary> GetDashboardSummaryAsync();
        Task<Dictionary<DateTime, decimal>> GetDailyRevenueAsync(DateTime startDate, DateTime endDate);
        Task<List<SanPham>> GetTopSellingProductsAsync(int topN);
    }
}