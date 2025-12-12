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
    // Chỉ Manager (Role 2) mới được truy cập khu vực này
    [Authorize]
    [ManagerRole]
    public class AccountManagerController : Controller
    {
        private readonly IAccountManagerService _accountManagerService;

        public AccountManagerController(IAccountManagerService accountManagerService)
        {
            _accountManagerService = accountManagerService;
        }

        // =======================================================
        // 1. INDEX: Danh sách Khách hàng (Giữ nguyên)
        // =======================================================
        public async Task<IActionResult> Index()
        {
            var customers = await _accountManagerService.GetAllCustomersAsync();
            ViewData["Title"] = "Quản lý Khách hàng";
            return View(customers);
        }

        // =======================================================
        // 2. STAFFS: Danh sách Nhân viên/Shipper
        // =======================================================
        public async Task<IActionResult> Staffs()
        {
            var staffs = await _accountManagerService.GetAllStaffsAsync();
            ViewData["Title"] = "Quản lý Nhân viên/Shipper";
            return View(staffs);
        }

        // =======================================================
        // 3. CREATE STAFF (GET)
        // =======================================================
        public async Task<IActionResult> CreateStaff()
        {
            ViewBag.Roles = await _accountManagerService.GetAllRolesAsync();
            ViewData["Title"] = "Thêm Tài khoản Nội bộ mới";
            return View();
        }

        // 4. CREATE STAFF (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff([Bind("HoTen,Email,SoDienThoai,MatKhau,MaVaiTro,DiaChi")] TaiKhoan account)
        {
            if (string.IsNullOrWhiteSpace(account.MatKhau))
                ModelState.AddModelError("MatKhau", "Mật khẩu không được để trống.");

            if (ModelState.IsValid)
            {
                try
                {
                    await _accountManagerService.AddStaffAccountAsync(account);
                    TempData["Success"] = $"Đã thêm tài khoản '{account.HoTen}' thành công!";
                    return RedirectToAction(nameof(Staffs));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewBag.Roles = await _accountManagerService.GetAllRolesAsync();
            ViewData["Title"] = "Thêm Tài khoản Nội bộ mới";
            return View(account);
        }

        // =======================================================
        // 5. EDIT (GET): Sửa thông tin tài khoản (Cập nhật)
        // =======================================================
        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountManagerService.GetAccountByIdAsync(id);
            if (account == null) return NotFound();

            // Nếu là tài khoản nội bộ (Staff), cần load Roles để chỉnh sửa Role
            if (account.MaVaiTro >= 2 && account.MaVaiTro <= 4)
            {
                ViewBag.Roles = await _accountManagerService.GetAllRolesAsync();
            }

            ViewData["Title"] = $"Sửa Tài khoản: {account.HoTen}";
            return View(account);
        }

        // 6. EDIT (POST) (Cập nhật)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTk,HoTen,Email,SoDienThoai,DiaChi,TrangThai,MaVaiTro")] TaiKhoan account)
        {
            if (id != account.MaTk) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _accountManagerService.UpdateAccountInfoAsync(account);
                    TempData["Success"] = $"Đã cập nhật thông tin tài khoản '{account.HoTen}' thành công!";
                    // Chuyển hướng về Staffs nếu tài khoản là Staff
                    if (account.MaVaiTro >= 2 && account.MaVaiTro <= 4)
                        return RedirectToAction(nameof(Staffs));

                    return RedirectToAction(nameof(Index)); // Customer
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            // Nếu lỗi, load lại Roles nếu là Staff
            if (account.MaVaiTro >= 2 && account.MaVaiTro <= 4)
            {
                ViewBag.Roles = await _accountManagerService.GetAllRolesAsync();
            }

            ViewData["Title"] = $"Sửa Tài khoản: {account.HoTen}";
            return View(account);
        }

        // =======================================================
        // 7. CHANGE STATUS (Giữ nguyên)
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, bool status)
        {
            try
            {
                await _accountManagerService.UpdateAccountStatusAsync(id, status);
                TempData["Success"] = $"Đã {(status ? "kích hoạt" : "vô hiệu hóa")} tài khoản thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}