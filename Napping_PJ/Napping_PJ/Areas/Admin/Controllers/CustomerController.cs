using Microsoft.AspNetCore.Mvc;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public CustomerController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Customers);
        }
    }
}
