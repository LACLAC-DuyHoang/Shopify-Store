using Microsoft.AspNetCore.Mvc;

namespace SHOPIFY1.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Bảng điều khiển Nhân viên";
            return View();
        }
    }
}
