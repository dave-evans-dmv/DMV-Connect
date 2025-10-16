using DMVConnect.Controllers.Base;
using DMVConnect.Data.Helpers.Constants;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var reportedPosts = await _adminService.GetReportedPostsAsync();
            return View(reportedPosts ?? new List<Post>());
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetCount()
        {
            int reportCount = await _adminService.GetReportedPostCountAsync();
            return Ok(reportCount);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveReport(int postId)
        {
            await _adminService.ApproveReportAsync(postId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectReport(int postId)
        {
            await _adminService.RejectReportAsync(postId);
            return RedirectToAction("Index");
        }
    }
}
