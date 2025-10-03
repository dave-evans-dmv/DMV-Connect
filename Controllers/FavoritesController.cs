using DMVConnect.Controllers.Base;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    [Authorize]
    public class FavoritesController : BaseController
    {
        private readonly IPostService _postService;
        private readonly UserManager<User> _userManager;

        public FavoritesController(IPostService postService, UserManager<User> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var favoritePosts = await _postService.GetAllFavoritedPostsAsync(loggedInUserId.Value);
            return View(favoritePosts);
        }

        public async Task<IActionResult> NavigateToFavorites()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var favoritePosts = await _postService.GetAllFavoritedPostsAsync(loggedInUserId.Value);

            return RedirectToAction("Index", "Favorites");
        }
    }
}
