using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DMVConnect.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected int? GetUserId()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return null;
            }
            return int.Parse(loggedInUserId);
        }

        protected string? GetUserFullName()
        {
            return User.FindFirstValue(ClaimTypes.Name);
        }

        protected IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Authentication");
        }
    }
}
