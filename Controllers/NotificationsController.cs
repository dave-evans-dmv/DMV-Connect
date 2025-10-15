using DMVConnect.Controllers.Base;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService) 
        { 
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var userId = GetUserId();
            if (!userId.HasValue) RedirectToLogin();

            var count = await _notificationService.GetUnreadNotificationsCountAsync(userId.Value);
            return Json(count);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationsAsync()
        {
            var userId = GetUserId();
            if (!userId.HasValue) RedirectToLogin();
            var notifications = await _notificationService.GetNotificationsAsync(userId.Value);
            if (notifications.Count != null)
                return PartialView("Notifications/_Notifications", notifications);
            else
                return PartialView("Notifications/_Notifications", null); ;
        }

        [HttpPost]
        public async Task<IActionResult> SetNotificationAsRead(int notificationId)
        {
            await _notificationService.SetNotificationAsReadAsync(notificationId);

            var notifications = await _notificationService.GetNotificationsAsync(GetUserId().Value);
            return PartialView("Notifications/_Notifications", notifications);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNotification(int notificationId)
        {
            await _notificationService.RemoveNotificationAsync(notificationId);
            var notifications = await _notificationService.GetNotificationsAsync(GetUserId().Value);
            return PartialView("Notifications/_Notifications", notifications);
        }
    }
}
