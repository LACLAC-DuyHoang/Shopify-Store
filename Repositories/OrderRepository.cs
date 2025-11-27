// Repositories/OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;

namespace SHOPIFY1.Repositories
{
    public class OrderRepository : Repository<DonHang>, IOrderRepository
    {
        public OrderRepository(ShopifyContext context) : base(context) { }

        public async Task<DonHang?> GetOrderDetailsByIdAsync(int maDh)
        {
            return await _dbSet
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(d => d.MaDh == maDh);
        }

        public async Task AddCartItemsToOrderAsync(List<ChiTietDonHang> items)
        {
            await _context.ChiTietDonHangs.AddRangeAsync(items);
        }
    }
}