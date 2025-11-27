namespace SHOPIFY1.Services
{
    public interface IOrderService
    {
        Task<int> CheckoutAsync(int maKhachHang, string diaChi, string phuongThuc, int? couponId = null);
    }
}
