using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SHOPIFY1.Services
{
    public interface ICouponService
    {
        Task<List<MaGiamGium>> GetAllCouponsAsync();
        Task<MaGiamGium?> GetCouponByIdAsync(int id);
        Task AddCouponAsync(MaGiamGium coupon);
        Task UpdateCouponAsync(MaGiamGium coupon);
        Task DeleteCouponAsync(int id);
    }
}