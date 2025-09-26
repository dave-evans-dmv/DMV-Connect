using System.Diagnostics;
using DMVConnect.ViewModels.Home;
using DMVConnect.Data;
using DMVConnect.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMVConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        private int loggedInUserId = 2;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allPosts = await _context.Posts
                .Where(n => !n.isPrivate || n.UserId == loggedInUserId)
                .Include(n => n.User)
                .Include(l => l.Likes)
                .Include(c => c.Comments).ThenInclude(n => n.User)
                .Include(r => r.Reports)
                .Include(f => f.Favorites)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();

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

            // Check and save image
            if (post.Image != null && post.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (post.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/posts");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await post.Image.CopyToAsync(stream);

                    // Set the URL to the newPost object
                    newPost.ImageUrl = "/images/posts/" + fileName;
                }
            }

            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            // Check if user has already liked the post
            var like = await _context.Likes
                .Where(l => l.PostId == postLikeVM.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postLikeVM.PostId,
                    UserId = loggedInUserId
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            // Check if user has already favorited the post
            var favorite = await _context.Favorites
                .Where(l => l.PostId == postFavoriteVM.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postFavoriteVM.PostId,
                    UserId = loggedInUserId
                };
                await _context.Favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(l => l.Id == postVisibilityVM.PostId && l.UserId == loggedInUserId);

            if (post != null)
            {
                post.isPrivate = !post.isPrivate;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }

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

            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var newReport = new Report()
            {
                PostId = postReportVM.PostId,
                UserId = loggedInUserId,
                DateCreated = DateTime.Now
            };

            await _context.Reports.AddAsync(newReport);

            var post = await _context.Posts
                .FirstOrDefaultAsync(l => l.Id == postReportVM.PostId);

            if (post != null)
            {
                post.NrOfReports++;
                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePostComment(PostCommentDeleteVM postCommentDeleteVM)
        {
            var commentDb = await _context.Comments.FirstOrDefaultAsync(c => c.Id == postCommentDeleteVM.commentId);

            if (commentDb != null)
            {
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(PostDeleteVM postDeleteVM)
        {
            var post = await _context.Posts
                .Include(n => n.Likes)
                .Include(n => n.Favorites)
                .Include(n => n.Comments)
                .Include(n => n.Reports)
                .FirstOrDefaultAsync(c => c.Id == postDeleteVM.PostId);
            
            if (post != null)
            {
                _context.Likes.RemoveRange(post.Likes);
                _context.Favorites.RemoveRange(post.Favorites);
                _context.Comments.RemoveRange(post.Comments);
                _context.Reports.RemoveRange(post.Reports);

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
