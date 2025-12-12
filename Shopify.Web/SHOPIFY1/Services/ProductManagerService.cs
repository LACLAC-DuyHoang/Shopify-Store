// File: ProductManagerService.cs
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SHOPIFY1.Services
{
    public class ProductManagerService : IProductManagerService
    {
        private readonly ShopifyContext _context;

        public ProductManagerService(ShopifyContext context) => _context = context;

        // =======================================================
        // CRUD SẢN PHẨM (Giữ nguyên)
        // =======================================================
        // ... (Các phương thức CRUD Sản phẩm giữ nguyên)

        public async Task<List<SanPham>> GetAllProductsAsync()
        {
            return await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();
        }

        public async Task<SanPham> GetProductByIdAsync(int id)
        {
            return await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .FirstOrDefaultAsync(p => p.MaSp == id);
        }

        public async Task AddProductAsync(SanPham product)
        {
            product.NgayTao = DateTime.Now;
            product.NgayCapNhat = DateTime.Now;
            if (!product.TrangThai.HasValue) product.TrangThai = true;

            _context.SanPhams.Add(product);
            await _context.SaveChangesAsync();
        }

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
        // QUẢN LÝ DANH MỤC (Cập nhật logic)
        // =======================================================

        public async Task<List<DanhMuc>> GetAllCategoriesAsync()
        {
            return await _context.DanhMucs.OrderBy(d => d.TenDm).ToListAsync();
        }

        public async Task<DanhMuc?> GetCategoryByIdAsync(int id)
        {
            return await _context.DanhMucs.FindAsync(id);
        }

        public async Task AddCategoryAsync(DanhMuc category)
        {
            if (await _context.DanhMucs.AnyAsync(d => d.TenDm == category.TenDm))
                throw new Exception($"Danh mục '{category.TenDm}' đã tồn tại.");

            category.NgayTao = DateTime.Now;
            _context.DanhMucs.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(DanhMuc category)
        {
            var existing = await _context.DanhMucs.FindAsync(category.MaDm);
            if (existing == null) throw new Exception("Danh mục không tồn tại.");

            // Kiểm tra trùng tên (trừ chính nó)
            if (await _context.DanhMucs.AnyAsync(d => d.TenDm == category.TenDm && d.MaDm != category.MaDm))
                throw new Exception($"Tên danh mục '{category.TenDm}' đã được sử dụng.");

            existing.TenDm = category.TenDm;
            existing.MoTa = category.MoTa;
            // NgayTao giữ nguyên

            _context.DanhMucs.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.DanhMucs.FindAsync(id);
            if (category == null)
                throw new Exception("Danh mục không tồn tại.");

            // KIỂM TRA RÀNG BUỘC
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