using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using Newtonsoft.Json;

namespace Napping_PJ.Controllers
{
	public class CheckOutController : Controller
	{
		public db_a989f8_nappingContext _Context { get; private set; }
		public CheckOutController(db_a989f8_nappingContext context)
		{
			_Context = context;
		}
		public async Task<IActionResult> Index()
		{
			string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
			Customer loginedUsr = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmail);
			byte[] cartBytes = HttpContext.Session.Get($"{loginedUsr.CustomerId}_cartItem");
			if (cartBytes == null)
			{
				return BadRequest("購物車為空");
			}
			string json = System.Text.Encoding.UTF8.GetString(cartBytes);
			IEnumerable<RoomDetailViewModel> roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
			return View(roomDetailViewModels);
		}
		
	}
}
