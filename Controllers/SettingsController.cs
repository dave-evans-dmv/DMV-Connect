using DMVConnect.Controllers.Base;
using DMVConnect.Data.Helpers.Enums;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using DMVConnect.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public SettingsController(
            IUserService userService, 
            IFileService fileService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userService = userService;
            _fileService = fileService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            return View(loggedInUser);
        }

        public async Task<IActionResult> NavigateToSettings()
        {
            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureVM profilePictureVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var uploadedProfilePictureUrl = await _fileService.UploadImageAsync(profilePictureVM.ProfilePicture, ImageFileType.ProfileImage);

            await _userService.UpdateUserProfilePicture(loggedInUserId.Value, uploadedProfilePictureUrl);

            return RedirectToAction("Index");

        }
    }
}
