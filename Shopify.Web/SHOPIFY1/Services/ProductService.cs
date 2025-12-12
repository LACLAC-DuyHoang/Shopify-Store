using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;

namespace SHOPIFY1.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopifyContext _context;

        public ProductService(ShopifyContext context)
        {
            _context = context;
        }

        public async Task<List<SanPham>> GetLatestAsync(int count)
        {
            return await _context.SanPhams
                .OrderByDescending(p => p.MaSp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<SanPham> GetByIdAsync(int id)
        {
            return await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .FirstOrDefaultAsync(p => p.MaSp == id);
        }

        public async Task<List<SanPham>> SearchByNameAsync(string keyword, int count)
        {
            return await _context.SanPhams
                .Where(p => EF.Functions.Like(p.TenSp, $"%{keyword}%"))
                .OrderByDescending(p => p.MaSp)
                .Take(count)
                .ToListAsync();
        }

        // MỚI: Lọc theo danh mục + giá + tìm kiếm
        public async Task<List<SanPham>> GetByCategoryAsync(int? categoryId, string priceRange, string search, int count)
        {
            var query = _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => EF.Functions.Like(p.TenSp, $"%{search.Trim()}%"));
            }

            // Lọc danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.MaDm == categoryId.Value);
            }

            // Lọc giá
            if (!string.IsNullOrEmpty(priceRange))
            {
                query = priceRange switch
                {
                    "0-10" => query.Where(p => p.Gia >= 0 && p.Gia <= 10_000_000),
                    "10-20" => query.Where(p => p.Gia > 10_000_000 && p.Gia <= 20_000_000),
                    "20-30" => query.Where(p => p.Gia > 20_000_000 && p.Gia <= 30_000_000),
                    "30+" => query.Where(p => p.Gia > 30_000_000),
                    _ => query
                };
            }

            return await query
                .OrderByDescending(p => p.MaSp)
                .Take(count)
                .ToListAsync();
        }

    }
}
