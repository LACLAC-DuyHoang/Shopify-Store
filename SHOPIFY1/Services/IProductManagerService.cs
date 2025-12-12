// File: IProductManagerService.cs
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SHOPIFY1.Services
{
    public interface IProductManagerService
    {
        // CRUD Sản phẩm (giữ nguyên)
        Task<List<SanPham>> GetAllProductsAsync();
        Task<SanPham> GetProductByIdAsync(int id);
        Task AddProductAsync(SanPham product);
        Task UpdateProductAsync(SanPham product);
        Task DeleteProductAsync(int id); // Xóa mềm (vô hiệu hóa)

        // Quản lý Danh mục (Cập nhật)
        Task<List<DanhMuc>> GetAllCategoriesAsync();
        Task<DanhMuc?> GetCategoryByIdAsync(int id); // Thêm: Lấy theo ID
        Task AddCategoryAsync(DanhMuc category); // Thêm: Thêm mới
        Task UpdateCategoryAsync(DanhMuc category); // Thêm: Cập nhật
        Task DeleteCategoryAsync(int id); // Xóa cứng (có kiểm tra ràng buộc)
    }
}