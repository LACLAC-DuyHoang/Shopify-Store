using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SHOPIFY1.Services
{
    public interface IEmployeeService
    {
        // Lấy danh sách đơn hàng đang chờ xử lý
        Task<List<DonHang>> GetPendingOrdersAsync();

        // Lấy chi tiết đơn hàng (bao gồm cả chi tiết sản phẩm)
        Task<DonHang?> GetOrderDetailsAsync(int orderId);

        // Cập nhật trạng thái đơn hàng (Xác nhận, Hủy)
        Task UpdateOrderStatusAsync(int orderId, string newStatus);

        // Lấy danh sách Shipper có sẵn (Vai trò 4)
        Task<List<TaiKhoan>> GetAvailableShippersAsync();

        // Gán Shipper cho đơn hàng
        Task AssignShipperAsync(int orderId, int shipperId);
    }
}