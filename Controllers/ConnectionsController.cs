using DMVConnect.Controllers.Base;
using DMVConnect.Data.Interfaces;
using DMVConnect.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace DMVConnect.Controllers
{
    public class ConnectionsController : BaseController
    {
        public readonly IConnectionsService _connectionsService;

        public ConnectionsController(IConnectionsService connectionsService)
        {
            _connectionsService = connectionsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NavigateToConnections()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            return RedirectToAction("Index", "Connections");
        }
    }
}
