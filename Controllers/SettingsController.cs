using DMVConnect.Data;
using DMVConnect.Data.Helpers.Enums;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Services;
using DMVConnect.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DMVConnect.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        private int loggedInUserId = 2;

        private int loggedInUser = 2;
        public SettingsController(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var userDb = await _userService.GetUser(loggedInUser);
            return View(userDb);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureVM profilePictureVM)
        {
            var uploadedProfilePictureUrl = await _fileService.UploadImageAsync(profilePictureVM.ProfilePicture, ImageFileType.ProfileImage);

            await _userService.UpdateUserProfilePicture(loggedInUserId, uploadedProfilePictureUrl);

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UpdateProfileVM profileVM)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPassword(UpdatePasswordVM passwordVM)
        {
            return RedirectToAction("Index");
        }
    }
}
