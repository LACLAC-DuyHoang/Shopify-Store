using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SHOPIFY1.Filters; // Cần có Filter này
using SHOPIFY1.Services;
using System.Threading.Tasks;
using System;
using SHOPIFY1.Models; // Cần Model DonHang

namespace SHOPIFY1.Controllers
{
    [Authorize] // Bắt buộc đăng nhập
    [OrderManagerRole] // Chỉ ROLE 3 mới vào được
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // Dashboard hiện tại giữ nguyên
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Bảng điều khiển Nhân viên";
            return View();
        }

        // =======================================================
        // 1. DANH SÁCH ĐƠN HÀNG CẦN XỬ LÝ (Action gây ra lỗi 404)
        // =======================================================
        public async Task<IActionResult> PendingOrders()
        {
            ViewData["Title"] = "Đơn hàng Chờ xử lý";
            var orders = await _employeeService.GetPendingOrdersAsync();
            return View(orders);
        }

        // =======================================================
        // 2. CHI TIẾT ĐƠN HÀNG
        // =======================================================
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _employeeService.GetOrderDetailsAsync(id);
            if (order == null) return NotFound();

            // Lấy danh sách Shipper để gán đơn hàng
            ViewBag.Shippers = await _employeeService.GetAvailableShippersAsync();

            ViewData["Title"] = $"Chi tiết Đơn hàng: #{id}";
            return View(order);
        }

        // =======================================================
        // 3. CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG (Xác nhận/Hủy)
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int maDh, string trangThaiMoi)
        {
            try
            {
                await _employeeService.UpdateOrderStatusAsync(maDh, trangThaiMoi);
                TempData["Success"] = $"Đơn hàng #{maDh} đã được cập nhật trạng thái thành: **{trangThaiMoi}**";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cập nhật thất bại: {ex.Message}";
            }

            return RedirectToAction(nameof(PendingOrders));
        }

        // =======================================================
        // 4. GÁN SHIPPER CHO ĐƠN HÀNG
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignShipper(int maDh, int maShipper)
        {
            try
            {
                await _employeeService.AssignShipperAsync(maDh, maShipper);
                TempData["Success"] = $"Đơn hàng #{maDh} đã được gán cho Shipper.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Gán Shipper thất bại: {ex.Message}";
            }

            return RedirectToAction(nameof(OrderDetails), new { id = maDh });
        }
    }
}