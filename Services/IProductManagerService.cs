// File: IProductManagerService.cs
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SHOPIFY1.Services
{
    public interface IProductManagerService
    {
        // CRUD Sản phẩm
        Task<List<SanPham>> GetAllProductsAsync();
        Task<SanPham> GetProductByIdAsync(int id);
        Task AddProductAsync(SanPham product);
        Task UpdateProductAsync(SanPham product);
        Task DeleteProductAsync(int id); // Xóa mềm (vô hiệu hóa)

        // Quản lý Danh mục
        Task<List<DanhMuc>> GetAllCategoriesAsync();
        Task DeleteCategoryAsync(int id); // Xóa cứng (có kiểm tra ràng buộc)
    }
}