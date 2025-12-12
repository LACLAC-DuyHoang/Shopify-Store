namespace SHOPIFY1.Services
{
    public interface IOrderService
    {
        Task<int> CheckoutAsync(int maKhachHang, string diaChi, string phuongThuc, int? couponId = null);

        // Thêm: Lấy danh sách đơn hàng theo Khách hàng
        Task<List<SHOPIFY1.Models.DonHang>> GetOrdersByCustomerAsync(int maKhachHang);

        // Thêm: Lấy chi tiết đơn hàng theo ID và đảm bảo thuộc về Khách hàng đó
        Task<SHOPIFY1.Models.DonHang?> GetOrderDetailForCustomerAsync(int orderId, int maKhachHang);
    }
}