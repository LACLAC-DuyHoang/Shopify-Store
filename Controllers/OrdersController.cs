using Microsoft.AspNetCore.Mvc;
using SHOPIFY1.Services;

namespace SHOPIFY1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;


        public async Task<IActionResult> Checkout(int userId)
        {
            // For demo, use fixed address & payment method
            var orderId = await _orderService.CheckoutAsync(userId, "Hà Nội", "COD");
            return RedirectToAction("Details", new { id = orderId });
        }


        public IActionResult Details(int id)
        {
            // load order + details and display
            return View(id);
        }
    }
}
