// Repositories/IOrderRepository.cs
using SHOPIFY1.Models;

namespace SHOPIFY1.Repositories
{
    public interface IOrderRepository : IRepository<DonHang>
    {
        // Có thể thêm các phương thức tìm kiếm đơn hàng phức tạp ở đây
        Task<DonHang?> GetOrderDetailsByIdAsync(int maDh);

        // Cần cho logic Checkout (những thao tác nhỏ được giữ trong Repository để đảm bảo DI)
        Task AddCartItemsToOrderAsync(List<ChiTietDonHang> items);
    }
}