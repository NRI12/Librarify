using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace libraryproject.Attributes
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(string role = "") : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { role };
        }
    }

    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly string _role;

        public AuthorizeActionFilter(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var userRole = context.HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                // Người dùng chưa đăng nhập, chuyển đến trang đăng nhập
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!string.IsNullOrEmpty(_role) && _role != userRole)
            {
                // Người dùng đã đăng nhập nhưng không có quyền truy cập
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}