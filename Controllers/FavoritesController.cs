using DMVConnect.Data.Models;
using DMVConnect.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DMVConnect.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly IPostService _postService;

        private int loggedInUser = 2;

        public FavoritesController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var favoritePosts = await _postService.GetAllFavoritedPostsAsync(loggedInUser);
            return View(favoritePosts);
        }
    }
}
