// Filters/AdminRoleAttribute.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SHOPIFY1.Filters
{
    public class AdminRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetInt32("UserRole");
            if (role != 2)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
            base.OnActionExecuting(context);
        }
    }
}