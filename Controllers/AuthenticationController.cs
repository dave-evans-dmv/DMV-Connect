using DMVConnect.Data.Helpers.Constants;
using DMVConnect.Data.Models;
using DMVConnect.ViewModels.Authentication;
using DMVConnect.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DMVConnect.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var newUser = new User
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                FullName = $"{registerVM.FirstName} {registerVM.LastName}"
            };

            var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
            if (existingUser != null) 
            {
                ModelState.AddModelError("Email", "Email already exists. Please login instead.");
                return View(registerVM);
            }

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);
                await _userManager.AddClaimAsync(newUser, new Claim(UserCustomClaims.FullName, newUser.FullName));
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var existingUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid email or password. Please, try again.");

                return View(loginVM);
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(existingUser);
            if (!existingUserClaims.Any(c => c.Type == UserCustomClaims.FullName))
            {
                await _userManager.AddClaimAsync(existingUser, new Claim(UserCustomClaims.FullName, existingUser.FullName));
            }

            var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");

            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPassword(UpdatePasswordVM updatePasswordVM)
        {

            if (updatePasswordVM.NewPassword != updatePasswordVM.ConfirmPassword)
            {
                TempData["PasswordError"] = "Passwords do not match";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }

            var loggedInUser = await _userManager.GetUserAsync(User);
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(loggedInUser, updatePasswordVM.CurrentPassword);

            if (!isCurrentPasswordValid)
            {
                TempData["PasswordError"] = "Current password is invalid";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }

            if (updatePasswordVM.CurrentPassword == updatePasswordVM.NewPassword)
            {
                TempData["PasswordError"] = "Current password and new password are the same";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }

            var result = await _userManager.ChangePasswordAsync(loggedInUser, updatePasswordVM.CurrentPassword, updatePasswordVM.NewPassword);

            if (result.Succeeded)
            {
                TempData["PasswordSuccess"] = "Password updated successfully";
                TempData["ActiveTab"] = "Password";

                await _signInManager.RefreshSignInAsync(loggedInUser);
            }
            else
            {
                TempData["ActiveTab"] = "Password";

                foreach (var error in result.Errors)
                {
                    TempData["PasswordError"] += $"New {error.Description}<br/>";
                }
            }
            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(UpdateProfileVM profileVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            user.FullName = profileVM.FullName;
            user.UserName = profileVM.Username;
            user.Bio = profileVM.Bio;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["UserProfileError"] = "User profile could not be updated";
                TempData["ActiveTab"] = "Profile";

                return RedirectToAction("Index", "Settings");
            }

            TempData["ProfileSuccess"] = "User profile updated successfully";
            TempData["ActiveTab"] = "Profile";

            return RedirectToAction("Index", "Settings");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
