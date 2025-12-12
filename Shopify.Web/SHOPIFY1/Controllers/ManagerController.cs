using Microsoft.AspNetCore.Mvc;
using SHOPIFY1.Models;
using SHOPIFY1.Services;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using SHOPIFY1.Filters; // Thêm thư viện Filters

namespace SHOPIFY1.Controllers
{
    // Controller này dành cho vai trò Manager (MaVaiTro = 2)
    [Authorize] // Bắt buộc đăng nhập
    [ManagerRole] // Chỉ Role 2 (Manager) mới được truy cập
    public class ManagerController : Controller
    {
        private readonly IProductManagerService _productManagerService;
        private readonly IManagementDashboardService _dashboardService; // ✅ Thêm Dependency cho Dashboard Service

        // Dependency Injection cho Service Quản lý Sản phẩm
        public ManagerController(IProductManagerService productManagerService, IManagementDashboardService dashboardService)
        {
            _productManagerService = productManagerService;
            _dashboardService = dashboardService; // ✅ Khởi tạo Dashboard Service
        }

        // =======================================================
        // 0. DASHBOARD (Cập nhật để lấy dữ liệu động)
        // =======================================================
        public async Task<IActionResult> Dashboard()
        {
            // Lấy dữ liệu thống kê từ Service
            var stats = await _dashboardService.GetDashboardStatsAsync();

            // Gán dữ liệu vào ViewBag để View (Dashboard.cshtml) có thể sử dụng
            ViewBag.TotalProducts = stats["TotalProducts"];
            ViewBag.PendingOrders = stats["PendingOrders"];
            ViewBag.MonthlyRevenue = stats["MonthlyRevenue"];

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

        // =======================================================
        // 6. THÊM DANH MỤC (CREATE)
        // =======================================================
        public IActionResult CreateCategory()
        {
            ViewData["Title"] = "Thêm Danh mục mới";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("TenDm,MoTa")] DanhMuc category)
        {
            if (string.IsNullOrWhiteSpace(category.TenDm))
            {
                ModelState.AddModelError("TenDm", "Tên danh mục không được để trống.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productManagerService.AddCategoryAsync(category);
                    TempData["Success"] = $"Đã thêm danh mục '{category.TenDm}' thành công!";
                    return RedirectToAction(nameof(Categories));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewData["Title"] = "Thêm Danh mục mới";
            return View(category);
        }

        // =======================================================
        // 7. SỬA DANH MỤC (EDIT)
        // =======================================================
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _productManagerService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            ViewData["Title"] = $"Sửa Danh mục: {category.TenDm}";
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, [Bind("MaDm,TenDm,MoTa,NgayTao")] DanhMuc category)
        {
            if (id != category.MaDm) return NotFound();

            if (string.IsNullOrWhiteSpace(category.TenDm))
            {
                ModelState.AddModelError("TenDm", "Tên danh mục không được để trống.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productManagerService.UpdateCategoryAsync(category);
                    TempData["Success"] = $"Đã cập nhật danh mục '{category.TenDm}' thành công!";
                    return RedirectToAction(nameof(Categories));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewData["Title"] = $"Sửa Danh mục: {category.TenDm}";
            return View(category);
        }

        // =======================================================
        // 8. XÓA DANH MỤC
        // =======================================================
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