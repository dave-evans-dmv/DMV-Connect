using DMVConnect.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMVConnect.ViewComponents
{
    public class HashtagsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        
        public HashtagsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var oneWeekAgoNow = DateTime.Now.AddDays(-7);
            var topTags = await _context.Hashtags
                .Where(h => h.DateCreated >= oneWeekAgoNow)
                .Where(h => h.Count > 0)
                .OrderByDescending(n => n.Count)
                .Take(3)
                .ToListAsync();

            return View(topTags);
        }
    }
}
