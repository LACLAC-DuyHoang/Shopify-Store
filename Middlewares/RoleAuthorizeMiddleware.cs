namespace SHOPIFY1.Middlewares
{
    public class RoleAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // 📌 BỎ QUA kiểm tra cho các trang công khai và static files
            // Hệ thống Cookie Authentication sẽ lo việc chuyển hướng về Login
            if (path == null ||
                path.StartsWith("/account/login") ||
                path.StartsWith("/account/register") ||
                path.StartsWith("/account/logout") ||
                path.StartsWith("/product") || // Bỏ qua trang sản phẩm công khai
                path.StartsWith("/home") || // Bỏ qua trang chủ
                path.StartsWith("/error") ||
                path.Contains(".")) // Bỏ qua file tĩnh (css, js, images)
            {
                await _next(context);
                return;
            }

            // Lấy vai trò trong Session (vẫn cần cho logic tùy chỉnh)
            var roleId = context.Session.GetInt32("MaVaiTro");

            // Kiểm tra các vùng bảo mật cao
            if (path.StartsWith("/admin") || path.StartsWith("/employee") || path.StartsWith("/shipper"))
            {
                // Sau khi thêm Cookie Auth, chúng ta không cần kiểm tra 'roleId == null' nữa
                // vì [Authorize] và Cookie Auth sẽ chuyển hướng người dùng chưa đăng nhập.
                // Chúng ta chỉ cần kiểm tra: Nếu người dùng đã đăng nhập nhưng Role không hợp lệ

                if (roleId == null)
                {
                    // Nếu đến đây mà chưa có Role trong Session, có thể là do Session bị mất
                    // Hoặc người dùng đang cố gắng truy cập mà không có claims/session.
                    // Chúng ta nên để Cookie Auth xử lý, nhưng để đảm bảo, chuyển hướng về Access Denied.
                    // Hoặc, tin tưởng Cookie Auth và chỉ kiểm tra quyền

                    // Nếu người dùng đã được xác thực (có cookie) nhưng MaVaiTro trong Session bị mất, 
                    // chúng ta có thể chuyển hướng về Access Denied để buộc họ tái xác thực hoặc báo lỗi.
                    if (context.User.Identity?.IsAuthenticated ?? false)
                    {
                        // Người dùng đăng nhập nhưng không có MaVaiTro trong Session (lỗi session/caching)
                        context.Response.Redirect("/Home/AccessDenied");
                        return;
                    }

                    // Để tránh lỗi lặp, chúng ta chỉ kiểm tra quyền role cụ thể:
                    // Kiểm tra quyền hợp lệ
                    if (path.StartsWith("/admin") && roleId != 2)
                    {
                        context.Response.Redirect("/Home/AccessDenied");
                        return;
                    }
                    if (path.StartsWith("/employee") && roleId != 3)
                    {
                        context.Response.Redirect("/Home/AccessDenied");
                        return;
                    }
                    if (path.StartsWith("/shipper") && roleId != 4)
                    {
                        context.Response.Redirect("/Home/AccessDenied");
                        return;
                    }
                }
            }

            await _next(context);
        }

        // ... (RoleAuthorizeMiddlewareExtensions giữ nguyên)
    }

    public static class RoleAuthorizeMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleAuthorize(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleAuthorizeMiddleware>();
        }
    }
}