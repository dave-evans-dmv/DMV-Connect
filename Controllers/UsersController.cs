using DMVConnect.Controllers.Base;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using DMVConnect.Data.Services;
using DMVConnect.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    public class UsersController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IConnectionsService _connectionsService;

        public UsersController(IUserService userService, UserManager<User> userManager,
            IConnectionsService connectionsService)
        {
            _userService = userService;
            _userManager = userManager;
            _connectionsService = connectionsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int userId)
        {
            var userData = await _userManager.FindByIdAsync(userId.ToString());
            var userPosts = await _userService.GetUserPosts(userId);
            var connections = await _connectionsService.GetConnectionsAsync(userId);

            var UserProfileVM = new GetUserProfileVM()
            {
                Connections = connections,
                User = userData,
                Posts = userPosts
            };

            return View(UserProfileVM);
        }
    }
}
