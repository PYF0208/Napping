using Microsoft.AspNetCore.Mvc;

namespace Napping_PJ.Controllers
{
	public class GoldFlowController : Controller
	{
		public IActionResult Success()
		{
			return View();
		}
		public IActionResult Error()
		{
			return View();
		}
	}
}
