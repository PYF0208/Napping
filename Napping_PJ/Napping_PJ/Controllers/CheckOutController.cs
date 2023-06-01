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
		public async Task<string> SaveOrder()
		{
			Claim userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
			if (userEmailClaim == null)
			{
				return "找不到使用者";
			}
			var user = _Context.Customers.FirstOrDefault(x => x.Email == userEmailClaim.Value);
			Order order = new Order
			{
				CustomerId = user.CustomerId,
				Date = DateTime.Now,
				PaymentId=1,
			};
			_Context.Orders.Add(order);
			_Context.SaveChanges();
			
			
			OrderDetail Detail = new OrderDetail
			{
				CheckIn = DateTime.Now,
				CheckOut = DateTime.MaxValue,
				RoomId = 140,
				ProfitId=5,
				OrderId =order.OrderId,
				NumberOfGuests=5,
				TravelType ="放鬆旅遊" ,
			};
			_Context.OrderDetails.Add(Detail);
			_Context.SaveChanges();
			OrderDetailExtraService ODE = new OrderDetailExtraService 
			{
				OrderDetailId=Detail.OrderDetailId,
				ExtraServiceName="按摩",
				Number=1,

			};
			return "成功";


		}
	}
}
