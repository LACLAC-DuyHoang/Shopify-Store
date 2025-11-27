// Repositories/ProductRepository.cs
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Linq;

namespace SHOPIFY1.Repositories
{
    public class ProductRepository : Repository<SanPham>, IProductRepository
    {
        public ProductRepository(ShopifyContext context) : base(context) { }

        public async Task<SanPham?> GetProductDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.MaDmNavigation)
                .FirstOrDefaultAsync(p => p.MaSp == id);
        }

        public async Task<IEnumerable<SanPham>> GetProductsByFilterAsync(int? categoryId, string? priceRange, string? search, int take = 50)
        {
            var query = _dbSet
                .Include(p => p.MaDmNavigation)
                .Where(p => p.TrangThai == true)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.MaDm == categoryId.Value);
            }

            // [Tạm bỏ qua logic lọc giá phức tạp ở đây để giữ mã đơn giản]

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.TenSp != null && p.TenSp.Contains(search));
            }

            return await query
                .OrderByDescending(p => p.NgayTao)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<SanPham>> GetLatestProductsAsync(int count)
        {
            return await _dbSet
                .Where(p => p.TrangThai == true)
                .OrderByDescending(p => p.NgayTao)
                .Take(count)
                .ToListAsync();
        }
    }
}