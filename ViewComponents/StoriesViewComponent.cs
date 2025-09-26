using DMVConnect.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMVConnect.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public StoriesViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _context.Stories
                .Include(n => n.User)
                .Where(n => n.DateCreated >= DateTime.Now.AddHours(-24))
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();

            return View(allStories);
        }
    }
}
