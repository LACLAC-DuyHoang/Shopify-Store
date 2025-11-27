using Microsoft.AspNetCore.Mvc;
using SHOPIFY1.Models;
using SHOPIFY1.Services;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization; // Có thể cần nếu dùng Authorize attribute

namespace SHOPIFY1.Controllers
{
    // Controller này dành cho vai trò Manager (MaVaiTro = 2)
    public class ManagerController : Controller
    {
        private readonly IProductManagerService _productManagerService;

        // Dependency Injection cho Service Quản lý Sản phẩm
        public ManagerController(IProductManagerService productManagerService)
        {
            _productManagerService = productManagerService;
        }

        // =======================================================
        // 0. DASHBOARD (Mục tiêu ban đầu của bạn)
        // =======================================================
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Bảng điều khiển Quản lý";
            return View();
        }

        // =======================================================
        // 1. QUẢN LÝ SẢN PHẨM (INDEX)
        // =======================================================
        public async Task<IActionResult> Products()
        {
            var products = await _productManagerService.GetAllProductsAsync();
            ViewData["Title"] = "Quản lý Sản phẩm";
            return View(products);
        }

        // =======================================================
        // 2. THÊM SẢN PHẨM (CREATE)
        // =======================================================
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _productManagerService.GetAllCategoriesAsync();
            ViewData["Title"] = "Thêm Sản phẩm mới";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([Bind("TenSp,HangSx,Gia,MoTa,HinhAnh,SoLuongTon,MaDm,TrangThai")] SanPham product)
        {
            if (ModelState.IsValid)
            {
                await _productManagerService.AddProductAsync(product);
                TempData["Success"] = $"Đã thêm sản phẩm '{product.TenSp}' thành công!";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _productManagerService.GetAllCategoriesAsync();
            return View(product);
        }

        // =======================================================
        // 3. SỬA SẢN PHẨM (EDIT)
        // =======================================================
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productManagerService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _productManagerService.GetAllCategoriesAsync();
            ViewData["Title"] = $"Sửa Sản phẩm: {product.TenSp}";
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, [Bind("MaSp,TenSp,HangSx,Gia,MoTa,HinhAnh,SoLuongTon,MaDm,TrangThai,NgayTao")] SanPham product)
        {
            if (id != product.MaSp) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _productManagerService.UpdateProductAsync(product);
                    TempData["Success"] = $"Đã cập nhật sản phẩm '{product.TenSp}' thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    ViewBag.Categories = await _productManagerService.GetAllCategoriesAsync();
                    return View(product);
                }
                return RedirectToAction(nameof(Products));
            }
            ViewBag.Categories = await _productManagerService.GetAllCategoriesAsync();
            return View(product);
        }

        // =======================================================
        // 4. XÓA SẢN PHẨM (DELETE - Cập nhật TrangThai = false)
        // =======================================================
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            await _productManagerService.DeleteProductAsync(id);
            TempData["Success"] = "Đã vô hiệu hóa sản phẩm thành công.";
            return RedirectToAction(nameof(Products));
        }

        // =======================================================
        // 5. QUẢN LÝ DANH MỤC (INDEX)
        // =======================================================
        public async Task<IActionResult> Categories()
        {
            var categories = await _productManagerService.GetAllCategoriesAsync();
            ViewData["Title"] = "Quản lý Danh mục";
            return View(categories);
        }

        // 6. XÓA DANH MỤC
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            try
            {
                await _productManagerService.DeleteCategoryAsync(id);
                TempData["Success"] = "Đã xóa danh mục thành công.";
            }
            catch (Exception ex)
            {
                // Bắt lỗi ràng buộc khóa ngoại từ Service
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Categories));
        }
    }
}