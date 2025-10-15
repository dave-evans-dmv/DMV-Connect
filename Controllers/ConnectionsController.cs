using DMVConnect.Controllers.Base;
using DMVConnect.Data.Helpers.Constants;
using DMVConnect.Data.Interfaces;
using DMVConnect.ViewModels.Connections;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    public class ConnectionsController : BaseController
    {
        public readonly IConnectionsService _connectionsService;
        private readonly INotificationService _notificationService;

        public ConnectionsController(
            IConnectionsService connectionsService, INotificationService notificationService)
        {
            _connectionsService = connectionsService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var connectionData = new ConnectionVM()
            {
                Connections = await _connectionsService.GetConnectionsAsync(loggedInUserId.Value),
                ConnectionRequestSent = await _connectionsService.GetSentConnectionsRequestAsync(loggedInUserId.Value),
                ConnectionRequestReceived = await _connectionsService.GetReceivedConnectionsRequestAsync(loggedInUserId.Value),
            };

            return View(connectionData);
        }

        public async Task<IActionResult> NavigateToConnections()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var connectionData = new ConnectionVM()
            {
                ConnectionRequestSent = await _connectionsService.GetSentConnectionsRequestAsync(loggedInUserId.Value)
            };

            return RedirectToAction("Index", "Connections", connectionData);
        }

        [HttpPost]
        public async Task<IActionResult> SendConnectionRequest(int receiverId)
        {
            var loggedInUserId = GetUserId();
            var userName = GetUserFullName();
            if (loggedInUserId == null) return RedirectToLogin();

            await _connectionsService.SendRequestAsync(loggedInUserId.Value, receiverId);

            await _notificationService.AddNewNotificationAsync(receiverId, loggedInUserId.Value, NotificationText.NotificationTypeConnectionRequest, userName, null);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveConnection(int connectionId )
        {
            await _connectionsService.RemoveConnectionAsync(connectionId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateConnectionRequest(int requestId, string status)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (!userId.HasValue) RedirectToLogin();

            var request = await _connectionsService.UpdateRequestAsync(requestId, status);

            if (status == ConnectionStatus.Accepted)
            {
                await _notificationService.AddNewNotificationAsync(request.SenderId, userId.Value, NotificationText.NotificationTypeAcceptedConnection, userName, null);
            }

            return RedirectToAction("Index");
        }
    }
}
