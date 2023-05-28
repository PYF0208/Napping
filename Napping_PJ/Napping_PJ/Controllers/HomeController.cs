using Microsoft.AspNetCore.Mvc;
using Napping_PJ.Models;
using System.Diagnostics;

namespace Napping_PJ.Controllers
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

        public IActionResult Result()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

		public IActionResult HotelPage(int id)
		{
            TempData["id"] = id;
			return View();
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}