using SHOPIFY1.Models;

namespace SHOPIFY1.Services
{
    public interface IProductService
    {
        Task<List<SanPham>> GetLatestAsync(int count);
        Task<SanPham> GetByIdAsync(int id);
        Task<List<SanPham>> SearchByNameAsync(string keyword, int count);
        Task<List<SanPham>> GetByCategoryAsync(int? categoryId, string priceRange, string search, int count);
    }
}
