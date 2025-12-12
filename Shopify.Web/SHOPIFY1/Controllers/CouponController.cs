using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SHOPIFY1.Filters;
using SHOPIFY1.Models;
using SHOPIFY1.Services;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SHOPIFY1.Controllers
{
    [Authorize]
    [ManagerRole] // Chỉ Manager (Role 2) mới được quản lý Coupon
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // =======================================================
        // 1. INDEX: Danh sách Mã Giảm Giá
        // =======================================================
        public async Task<IActionResult> Index()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            ViewData["Title"] = "Quản lý Mã Giảm Giá";
            return View(coupons);
        }

        // =======================================================
        // 2. CREATE (GET)
        // =======================================================
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Mã Giảm Giá mới";
            // Giá trị mặc định
            var model = new MaGiamGium
            {
                PhanTramGiam = 10,
                GiaTriToiDa = 1000000,
                SoLanSuDungToiDa = 0, // Không giới hạn
                NgayBatDau = DateTime.Now,
                NgayKetThuc = DateTime.Now.AddMonths(1),
            };
            return View(model);
        }

        // 3. CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCode,MoTa,PhanTramGiam,GiaTriToiDa,NgayBatDau,NgayKetThuc,SoLanSuDungToiDa")] MaGiamGium coupon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _couponService.AddCouponAsync(coupon);
                    TempData["Success"] = $"Đã thêm Mã Giảm Giá '{coupon.MaCode}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewData["Title"] = "Thêm Mã Giảm Giá mới";
            return View(coupon);
        }

        // =======================================================
        // 4. EDIT (GET)
        // =======================================================
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound();

            ViewData["Title"] = $"Sửa Mã Giảm Giá: {coupon.MaCode}";
            return View(coupon);
        }

        // 5. EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaMgg,MaCode,MoTa,PhanTramGiam,GiaTriToida,NgayBatDau,NgayKetThuc,SoLanSuDungToiDa,TrangThai")] MaGiamGium coupon)
        {
            if (id != coupon.MaMgg) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _couponService.UpdateCouponAsync(coupon);
                    TempData["Success"] = $"Đã cập nhật Mã Giảm Giá '{coupon.MaCode}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewData["Title"] = $"Sửa Mã Giảm Giá: {coupon.MaCode}";
            return View(coupon);
        }

        // =======================================================
        // 6. DELETE (Xóa mềm - Cập nhật trạng thái)
        // =======================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _couponService.DeleteCouponAsync(id);
                TempData["Success"] = "Đã vô hiệu hóa Mã Giảm Giá thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}