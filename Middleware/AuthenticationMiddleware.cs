using libraryproject.Data;
using Microsoft.EntityFrameworkCore;

namespace libraryproject.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, QLTVContext dbContext)
        {
            // Kiểm tra nếu user đã đăng nhập qua session
            if (context.Session.GetInt32("UserId") == null)
            {
                // Kiểm tra nếu có cookie đăng nhập
                if (context.Request.Cookies.TryGetValue("AuthToken", out string authToken))
                {
                    var user = await dbContext.NguoiDungs
                        .FirstOrDefaultAsync(u => u.AuthToken == authToken);

                    if (user != null)
                    {
                        // Khôi phục thông tin session
                        context.Session.SetInt32("UserId", user.ID);
                        context.Session.SetString("UserName", user.HoTen);
                        context.Session.SetString("UserRole", user.Role);
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method để thêm middleware vào pipeline
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthentication(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}