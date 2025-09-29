using DMVConnect.Data;
using DMVConnect.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DMVConnect.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUserService _userService;

        private int loggedInUser = 2;
        public SettingsController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userDb = await _userService.GetUser(loggedInUser);
            return View(userDb);
        }
    }
}
