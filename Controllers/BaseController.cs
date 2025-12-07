using Microsoft.AspNetCore.Mvc;

namespace StarSecurity.Controllers
{
    public class BaseController : Controller
    {
        protected string GetUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "";
        }

        protected int GetUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return userId != null ? int.Parse(userId) : 0;
        }
    }
}