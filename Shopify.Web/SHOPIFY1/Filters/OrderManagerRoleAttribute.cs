using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SHOPIFY1.Filters
{
    // Cho phép truy cập cho MaVaiTro = 2 (Manager) và 3 (Employee)
    public class OrderManagerRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetInt32("MaVaiTro");

            // Role hợp lệ là 2 (Manager) hoặc 3 (Employee)
            if (role != 2 && role != 3)
            {
                // Nếu không có quyền, chuyển hướng đến Access Denied
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}