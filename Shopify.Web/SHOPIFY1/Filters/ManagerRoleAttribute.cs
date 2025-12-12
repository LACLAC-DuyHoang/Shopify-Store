using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SHOPIFY1.Filters
{
    // Áp dụng cho MaVaiTro = 2 (Manager)
    public class ManagerRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetInt32("MaVaiTro");

            // Role Manager là 2
            if (role != 2)
            {
                // Chuyển hướng đến trang Access Denied
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}