using System.Diagnostics;
using DMV_Connect.Models;
using Microsoft.AspNetCore.Mvc;

namespace DMV_Connect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
