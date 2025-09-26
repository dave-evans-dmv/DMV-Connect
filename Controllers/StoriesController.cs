using DMVConnect.Data;
using DMVConnect.Data.Models;
using DMVConnect.ViewModels.Stories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMVConnect.Controllers
{
    public class StoriesController : Controller
    {
        private readonly AppDbContext _context;

        private int loggedInUserId = 2;

        public StoriesController(AppDbContext context) 
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var allStories = await _context.Stories
        //        .Include(n => n.User)
        //        .OrderByDescending(n => n.DateCreated)
        //        .ToListAsync();

        //    return View(allStories);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryVM story)
        {
            //Create new story
            var newStory = new Story
            {
                DateCreated = DateTime.Now,
                UserId = loggedInUserId
            };

            // Check and save image
            if (story.Image != null && story.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (story.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/stories");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(story.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await story.Image.CopyToAsync(stream);

                    // Set the URL to the newStory object
                    newStory.ImageUrl = "/images/stories/" + fileName;
                }
                await _context.Stories.AddAsync(newStory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
