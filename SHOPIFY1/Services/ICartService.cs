using SHOPIFY1.Models;

namespace SHOPIFY1.Services
{
    public interface ICartService
    {
        Task<GioHang> GetOrCreateCartAsync(int maKhachHang);
        Task AddToCartAsync(int maKhachHang, int maSP, int soLuong);
    }
}
