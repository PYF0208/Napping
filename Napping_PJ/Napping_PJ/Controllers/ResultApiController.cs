using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Security.Claims;

namespace Napping_PJ.Controllers
{
	[Route("api/Result/[action]")]
	[ApiController]
	public class ResultApiController : ControllerBase
	{
		private readonly db_a989f8_nappingContext _context;

		public ResultApiController(db_a989f8_nappingContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IEnumerable<ResultViewModel> Get()
		{
			HashSet<int> allHotel = null;
			if (User.Identity.IsAuthenticated)
			{
				var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
				allHotel = _context.Likes.AsNoTracking().Where(x => x.CustomerId == customer.CustomerId).Select(x => x.HotelId).ToHashSet();
			}

			var hotels = _context.Hotels.AsNoTracking()
				.Join(_context.Rooms, h => h.HotelId, r => r.HotelId, (h, r) => new { Hotel = h, Room = r }).ToList();


			return  hotels.GroupJoin(_context.OrderDetails, x => x.Room.RoomId, od => od.RoomId, (x, od) =>
				new ResultViewModel
				{
					HotelId = x.Hotel.HotelId,
					Name = x.Hotel.Name,
					Image = x.Hotel.Image,
					City = x.Hotel.City,
					Region = x.Hotel.Region,
					Price = x.Room.Price,
					RoomId = x.Room.RoomId,
					MaxGuests = x.Room.MaxGuests,
					PositionLat = (float)x.Hotel.PositionLat,
					PositionLon = (float)x.Hotel.PositionLon,
					IsLike = (allHotel != null && allHotel.Contains(x.Hotel.HotelId)) ? true : false,
					RoomOrders = od.Select(r => new RoomOrderViewModel
					{
						RoomId = r.RoomId,
						CheckIn = r.CheckIn,
						CheckOut = r.CheckOut,
					}).ToList()

				});

			
		}

		//[HttpGet]
		//public IEnumerable<ResultViewModel> GetCanBookRoom( rvm)
		//{
		//          var BookedRooms = _context.OrderDetails.Where(r => r.RoomId == rvm.RoomId)
		//              .Select(r => new ResultViewModel
		//		{
		//		RoomId = r.RoomId,
		//		CheckIn = r.CheckIn,
		//		CheckOut = r.CheckOut
		//	}); ;



		//}


	}
}
