using Microsoft.AspNetCore.Mvc;
using SHOPIFY1.Services;
using Microsoft.AspNetCore.Authorization;
using SHOPIFY1.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SHOPIFY1.Controllers
{
    [Authorize] // BẮT BUỘC ĐĂNG NHẬP
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        // Phương thức này có vẻ là demo hoặc bị bỏ qua
        // public async Task<IActionResult> Checkout(int userId)
        // {
        //     // For demo, use fixed address & payment method
        //     var orderId = await _orderService.CheckoutAsync(userId, "Hà Nội", "COD");
        //     return RedirectToAction("Details", new { id = orderId });
        // }

        // =======================================================
        // 1. INDEX: Danh sách đơn hàng của tôi
        // =======================================================
        // GET: /Orders/Index
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Logout", "Account"); // Hoặc AccessDenied

            var orders = await _orderService.GetOrdersByCustomerAsync(userId);
            ViewData["Title"] = "Đơn hàng của tôi";
            return View(orders);
        }

        // =======================================================
        // 2. DETAILS: Chi tiết đơn hàng
        // =======================================================
        // GET: /Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Logout", "Account");

            var order = await _orderService.GetOrderDetailForCustomerAsync(id, userId);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng hoặc đơn hàng này không thuộc về bạn.");

            ViewData["Title"] = $"Chi tiết Đơn hàng #{id}";
            return View(order);
        }
    }
}