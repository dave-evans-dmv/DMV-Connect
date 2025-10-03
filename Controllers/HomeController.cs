using DMVConnect.Controllers.Base;
using DMVConnect.Data.Helpers.Enums;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using DMVConnect.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly IFileService _fileService;
        private readonly IHashtagService _hashtagService;

        public HomeController(
            ILogger<HomeController> logger, 
            IPostService postService,
            IHashtagService hastagService,
            IFileService fileService)
        {
            _logger = logger;
            _postService = postService;
            _hashtagService = hastagService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var allPosts = await _postService.GetAllPostsync(loggedInUserId.Value);

            return View(allPosts);
        }

        public async Task<IActionResult> NavigateHome()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var allPosts = await _postService.GetAllPostsync(loggedInUserId.Value);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> PostDetails(int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            return View("PostDetails", post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM postVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var imageUploadPath = await _fileService.UploadImageAsync(postVM.Image, ImageFileType.PostImage);

            //Create new postVM
            var newPost = new Post
            {
                Content = postVM.Content,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                ImageUrl = imageUploadPath,
                NrOfReports = 0,
                UserId = loggedInUserId.Value
            };

            await _postService.CreatePostAsync(newPost);

            if (postVM.Content != null)
            {
                await _hashtagService.ProcessHashtagForNewPostAsync(postVM.Content);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUserId.Value);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUserId.Value);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, loggedInUserId.Value);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var newComment = new Comment()
            {
                PostId = postCommentVM.PostId,
                UserId = loggedInUserId.Value,
                Content = postCommentVM.Content,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            await _postService.AddPostCommentAsync(newComment, loggedInUserId.Value);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            await _postService.ReportPostAsync(postReportVM.PostId, loggedInUserId.Value);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(PostCommentDeleteVM postCommentDeleteVM)
        {
            await _postService.RemovePostCommentAsync(postCommentDeleteVM.commentId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(PostDeleteVM postDeleteVM)
        {
            var postDb = await _postService.DeletePostAsync(postDeleteVM.PostId);

            if (postDb.Content != null)
            {
                await _hashtagService.ProcessHashtagsForRemovedPostAsync(postDb.Content);
            }

            return RedirectToAction("Index");
        }
    }
}
