using DMVConnect.Data;
using DMVConnect.Data.Helpers.Enums;
using DMVConnect.Data.Models;
using DMVConnect.Data.Services;
using DMVConnect.ViewModels.Stories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMVConnect.Controllers
{
    public class StoriesController : Controller
    {
        private readonly IStoriesService _storiesService;
        private readonly IFileService _fileService;

        private int loggedInUserId = 2;

        public StoriesController(
            IStoriesService storiesService,
            IFileService fileService
        ) 
        {
            _storiesService = storiesService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var allStories = await _storiesService.GetAllStoriesAsync();

            return View(allStories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryVM storyVM)
        {
            var imageUploadPath = await _fileService.UploadImageAsync(storyVM.Image, ImageFileType.StoryImage);

            var newStory = new Story
            {
                DateCreated = DateTime.Now,
                UserId = loggedInUserId,
                ImageUrl = imageUploadPath
            };

            await _storiesService.CreateStoryAsync(newStory);

            return RedirectToAction("Index", "Home");
        }
    }
}
