using Microsoft.AspNetCore.Mvc;

namespace SHOPIFY1.Controllers
{
    public class ShipperController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Bảng điều khiển Shipper";
            return View();
        }
    }
}
