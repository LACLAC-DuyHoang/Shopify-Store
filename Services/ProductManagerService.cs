// File: ProductManagerService.cs
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SHOPIFY1.Services
{
    // Cần kế thừa IProductManagerService
    public class ProductManagerService : IProductManagerService
    {
        private readonly ShopifyContext _context;

        public ProductManagerService(ShopifyContext context) => _context = context;

        // =======================================================
        // CRUD SẢN PHẨM
        // =======================================================

        // Lấy tất cả Sản phẩm (bao gồm cả Danh mục để hiển thị)
        public async Task<List<SanPham>> GetAllProductsAsync()
        {
            return await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();
        }

        // Lấy Sản phẩm theo ID
        public async Task<SanPham> GetProductByIdAsync(int id)
        {
            return await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .FirstOrDefaultAsync(p => p.MaSp == id);
        }

        // Thêm Sản phẩm
        public async Task AddProductAsync(SanPham product)
        {
            product.NgayTao = DateTime.Now;
            product.NgayCapNhat = DateTime.Now;
            // Đảm bảo TrangThai được thiết lập (hoặc để mặc định theo DB)
            if (!product.TrangThai.HasValue) product.TrangThai = true;

            _context.SanPhams.Add(product);
            await _context.SaveChangesAsync();
        }

        // Cập nhật Sản phẩm
        public async Task UpdateProductAsync(SanPham product)
        {
            var existingProduct = await _context.SanPhams.FindAsync(product.MaSp);
            if (existingProduct == null)
                throw new Exception("Sản phẩm không tồn tại.");

            // Cập nhật các trường được phép
            existingProduct.TenSp = product.TenSp;
            existingProduct.HangSx = product.HangSx;
            existingProduct.Gia = product.Gia;
            existingProduct.MoTa = product.MoTa;
            existingProduct.HinhAnh = product.HinhAnh;
            existingProduct.SoLuongTon = product.SoLuongTon;
            existingProduct.MaDm = product.MaDm;
            existingProduct.TrangThai = product.TrangThai;
            existingProduct.NgayCapNhat = DateTime.Now;

            _context.SanPhams.Update(existingProduct);
            await _context.SaveChangesAsync();
        }

        // Xóa Sản phẩm (Chuyển sang trạng thái không hoạt động/Xóa mềm)
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.SanPhams.FindAsync(id);
            if (product != null)
            {
                product.TrangThai = false; // Vô hiệu hóa
                _context.SanPhams.Update(product);
                await _context.SaveChangesAsync();
            }
        }

        // =======================================================
        // QUẢN LÝ DANH MỤC
        // =======================================================

        // Lấy danh sách Danh mục
        public async Task<List<DanhMuc>> GetAllCategoriesAsync()
        {
            return await _context.DanhMucs.OrderBy(d => d.TenDm).ToListAsync();
        }

        // Xóa Danh mục (Xóa cứng kèm kiểm tra ràng buộc)
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.DanhMucs.FindAsync(id);
            if (category == null)
                throw new Exception("Danh mục không tồn tại.");

            // KIỂM TRA RÀNG BUỘC: Xem có Sản phẩm nào đang sử dụng Danh mục này không
            bool hasProducts = await _context.SanPhams.AnyAsync(p => p.MaDm == id);

            if (hasProducts)
            {
                throw new Exception($"Không thể xóa danh mục '{category.TenDm}'. Vẫn còn sản phẩm thuộc danh mục này.");
            }

            _context.DanhMucs.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}