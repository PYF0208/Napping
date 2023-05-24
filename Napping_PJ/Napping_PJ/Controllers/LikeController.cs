using Microsoft.AspNetCore.Mvc;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Controllers
{
    public class LikeController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public LikeController(db_a989f8_nappingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
