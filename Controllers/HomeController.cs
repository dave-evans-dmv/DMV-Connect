using DMVConnect.Controllers.Base;
using DMVConnect.Data.Helpers.Constants;
using DMVConnect.Data.Helpers.Enums;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using DMVConnect.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;

namespace DMVConnect.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly IFileService _fileService;
        private readonly IHashtagService _hashtagService;
        private readonly INotificationService _notificationService;

        public HomeController(
            ILogger<HomeController> logger, 
            IPostService postService,
            IHashtagService hastagService,
            IFileService fileService,
            INotificationService notificationService)
        {
            _logger = logger;
            _postService = postService;
            _hashtagService = hastagService;
            _fileService = fileService;
            _notificationService = notificationService;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            var loggedInUserId = GetUserId();
            var userName = GetUserFullName();
            if (loggedInUserId == null) return RedirectToLogin();

            var result = await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUserId.Value);
            var post = await _postService.GetPostByIdAsync(postLikeVM.PostId);

            if (result.SendNotification)
                await _notificationService.AddNewNotificationAsync(post.UserId, loggedInUserId.Value, NotificationText.NotificationTypeLike, userName, post.Id);

            return PartialView("Home/_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            var loggedInUserId = GetUserId();
            var userName = GetUserFullName();
            if (loggedInUserId == null) return RedirectToLogin();

            var result = await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUserId.Value);
            var post = await _postService.GetPostByIdAsync(postFavoriteVM.PostId);

            if (result.SendNotification)
                await _notificationService.AddNewNotificationAsync(post.UserId, loggedInUserId.Value, NotificationText.NotificationTypeBookmark, userName, postFavoriteVM.PostId);

            return PartialView("Home/_Post", post);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
        {
            var loggedInUserId = GetUserId();
            var userName = GetUserFullName();
            if (loggedInUserId == null) return RedirectToLogin();

            var newComment = new Comment()
            {
                PostId = postCommentVM.PostId,
                UserId = loggedInUserId.Value,
                Content = postCommentVM.Content,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            var result = await _postService.AddPostCommentAsync(newComment, loggedInUserId.Value);
            var post = await _postService.GetPostByIdAsync(postCommentVM.PostId);

            if (result.SendNotification)
                await _notificationService.AddNewNotificationAsync(post.UserId, loggedInUserId.Value, NotificationText.NotificationTypeComment, userName, post.Id);

            return PartialView("Home/_Post", post);
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var loggedInUserId = GetUserId();
            var userName = GetUserFullName();
            if (loggedInUserId == null) return RedirectToLogin();

            var result = await _postService.ReportPostAsync(postReportVM.PostId, loggedInUserId.Value);

            if (result.SendNotification)
                await _notificationService.AddNewNotificationAsync(postReportVM.UserId, loggedInUserId.Value, NotificationText.NotificationTypeReport, userName, postReportVM.PostId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePostComment(PostCommentDeleteVM postCommentDeleteVM)
        {
            var loggedInUserId = GetUserId();
            await _postService.RemovePostCommentAsync(postCommentDeleteVM.CommentId, loggedInUserId.Value);

            var post = await _postService.GetPostByIdAsync(postCommentDeleteVM.PostId);

            return PartialView("Home/_Post", post);
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
