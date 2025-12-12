using System.Threading.Tasks;
using System.Collections.Generic;

namespace SHOPIFY1.Services
{
    // Giao diện để lấy dữ liệu thống kê tổng quan
    public interface IManagementDashboardService
    {
        Task<Dictionary<string, object>> GetDashboardStatsAsync();
    }
}