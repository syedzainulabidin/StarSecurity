using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
namespace StarSecurity.Helpers
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Skip authorization if action is decorated with [AllowAnonymous]
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute)))
            {
                return;
            }

            var userRole = context.HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole) || !_allowedRoles.Contains(userRole))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}