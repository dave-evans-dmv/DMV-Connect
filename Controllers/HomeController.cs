using System.Diagnostics;
using DMVConnect.ViewModels.Home;
using DMVConnect.Data;
using DMVConnect.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DMVConnect.Data.Helpers;
using DMVConnect.Data.Helpers.Enums;
using DMVConnect.Data.Interfaces;

namespace DMVConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly IFileService _fileService;
        private readonly IHashtagService _hashtagService;

        private int loggedInUserId = 2;

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
            var allPosts = await _postService.GetAllPostsync(loggedInUserId);

            return View(allPosts);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM postVM)
        {
            var imageUploadPath = await _fileService.UploadImageAsync(postVM.Image, ImageFileType.PostImage);

            //Create new postVM
            var newPost = new Post
            {
                Content = postVM.Content,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                ImageUrl = imageUploadPath,
                NrOfReports = 0,
                UserId = loggedInUserId
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
            await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            var newComment = new Comment()
            {
                PostId = postCommentVM.PostId,
                UserId = loggedInUserId,
                Content = postCommentVM.Content,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            await _postService.AddPostCommentAsync(newComment, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            await _postService.ReportPostAsync(postReportVM.PostId, loggedInUserId);

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
