using DMVConnect.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DMVConnect.Controllers.Base;
using DMVConnect.ViewModels.Connections;

namespace DMVConnect.ViewComponents
{
    public class SuggestedConnectionsViewComponent : ViewComponent
    {
        private readonly IConnectionsService _connectionsService;

        public SuggestedConnectionsViewComponent(IConnectionsService connectionsService)
        {
            _connectionsService = connectionsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return null;
            }
            var userId = int.Parse(loggedInUserId);
            var suggestedConnections = await _connectionsService.GetSuggestedConnectionsAsync(userId);
            var suggestedConnectionsVM = suggestedConnections.Select(n => new UserWithConnectionsCountVM()
            {
                UserId = n.User.Id,
                FullName = n.User.FullName,
                ProfilePictureUrl = n.User.ProfilePictureUrl,
                ConnectionCount = n.ConnectionCount
            }).ToList();
            return View(suggestedConnectionsVM);
        }
    }
}
