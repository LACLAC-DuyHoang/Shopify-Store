using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SHOPIFY1.Filters
{
    // Áp dụng cho MaVaiTro = 3 (Employee)
    public class EmployeeRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetInt32("MaVaiTro");

            // Role Employee là 3
            if (role != 3)
            {
                // Nếu không phải Employee, chuyển hướng đến AccessDenied
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}