using DMVConnect.Controllers.Base;
using DMVConnect.Data.Helpers.Enums;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using DMVConnect.ViewModels.Stories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    [Authorize]
    public class StoriesController : BaseController
    {
        private readonly IStoriesService _storiesService;
        private readonly IFileService _fileService;

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
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var imageUploadPath = await _fileService.UploadImageAsync(storyVM.Image, ImageFileType.StoryImage);

            var newStory = new Story
            {
                DateCreated = DateTime.Now,
                UserId = loggedInUserId.Value,
                ImageUrl = imageUploadPath
            };

            await _storiesService.CreateStoryAsync(newStory);

            return RedirectToAction("Index", "Home");
        }
    }
}
