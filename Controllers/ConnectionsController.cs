using DMVConnect.Controllers.Base;
using DMVConnect.Data.Helpers.Constants;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Services;
using DMVConnect.ViewModels.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DMVConnect.Controllers
{
    public class ConnectionsController : BaseController
    {
        public readonly IConnectionsService _connectionsService;

        public ConnectionsController(IConnectionsService connectionsService)
        {
            _connectionsService = connectionsService;
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
            if (loggedInUserId == null) return RedirectToLogin();

            await _connectionsService.SendRequestAsync(loggedInUserId.Value, receiverId);

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
            await _connectionsService.UpdateRequestAsync(requestId, status);
            return RedirectToAction("Index");
        }
    }
}
