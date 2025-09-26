using System.Diagnostics;
using DMVConnect.ViewModels.Home;
using DMVConnect.Data;
using DMVConnect.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DMVConnect.Data.Helpers;
using DMVConnect.Data.Services;

namespace DMVConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IPostService _postService;

        private int loggedInUserId = 2;

        public HomeController(
            ILogger<HomeController> logger, 
            AppDbContext context, 
            IPostService postService)
        {
            _logger = logger;
            _context = context;
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var allPosts = await _postService.GetAllPostsync(loggedInUserId);

            return View(allPosts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            //Create new post
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                ImageUrl = "",
                NrOfReports = 0,
                UserId = loggedInUserId
            };

            await _postService.CreatePostAsync(newPost, post.Image);

            // Find & Store Hashtags
            var postHashtags = HashtagHelper.GetHashtags(post.Content);

            foreach (var tag in postHashtags)
            {
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Value == tag);

                if (hashtagDb != null)
                {
                    hashtagDb.Count++;
                    hashtagDb.DateUpdated = DateTime.Now;

                    _context.Hashtags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                } else
                {
                    var newTag = new Hashtag()
                    {
                        Value = tag,
                        Count = 1,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now
                    };

                    _context.Hashtags.Add(newTag);
                    await _context.SaveChangesAsync();
                }
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
            await _postService.DeletePostAsync(postDeleteVM.PostId);
            
            return RedirectToAction("Index");
        }
    }
}
