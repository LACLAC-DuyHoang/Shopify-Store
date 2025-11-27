// Repositories/IProductRepository.cs
using SHOPIFY1.Models;
using System.Collections.Generic;

namespace SHOPIFY1.Repositories
{
    public interface IProductRepository : IRepository<SanPham>
    {
        // Phương thức lọc phức tạp (dùng trong AdminController.Products)
        Task<IEnumerable<SanPham>> GetProductsByFilterAsync(int? categoryId, string? priceRange, string? search, int take = 50);

        // Lấy sản phẩm bao gồm cả Danh mục (dùng cho trang chi tiết)
        Task<SanPham?> GetProductDetailsAsync(int id);

        // Lấy sản phẩm mới nhất (dùng cho trang chủ)
        Task<IEnumerable<SanPham>> GetLatestProductsAsync(int count);
    }
}