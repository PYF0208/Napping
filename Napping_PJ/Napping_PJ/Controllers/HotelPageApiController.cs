using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Controllers
{
	[Route("api/HotelPage/[action]")]
	public class HotelPageApiController : Controller
	{
		private readonly db_a989f8_nappingContext _context;

		public HotelPageApiController(db_a989f8_nappingContext context)
		{
			_context = context;
		}

		[HttpGet("{id}")]
		public IEnumerable<HotelPageViewModel> Get(int id)
		{


			return _context.Hotels.Select(h => new HotelPageViewModel
			{
				HotelId = h.HotelId,
				Name = h.Name,
				Star = h.Star,
				Image = h.Image,
				ContactName = h.ContactName,
				Phone = h.Phone,
				City = h.City,
				Region = h.Region,
				Address = h.Address,
				Description = h.Description,

			}).Where(h => h.HotelId == id);

		}
		[HttpPost("{id}")]
		public object GetHotelsRoomType(int id) 
		{
			return _context.Hotels.Include(r => r.Rooms).ThenInclude(rimg => rimg.RoomImages).Where(h=>h.HotelId==id).Select(r => new
			{
				hotel = new
				{
				 Hotelid=r.HotelId,
				 Name=r.Name,
				 Star = r.Star,
				 Image = r.Image,
				 ContactName = r.ContactName,
				 Phone = r.Phone,
				 City = r.City,
				 Region = r.Region,
				 Address = r.Address,
				 Description = r.Description,
				},
				Room=r.Rooms,
				//img=r.Rooms.SelectMany(i=>i.RoomImages).ToList(),
			}).ToList() ;
		}

		[HttpPost("{id}")]
		public IEnumerable<HotelsViewModel> Testgett(int id)
		{
			return _context.Hotels.Select(r => new HotelsViewModel
			{
				HotelId = r.HotelId,
				Name = r.Name,
				Star = r.Star,
				Image = r.Image,
				ContactName = r.ContactName,
				Phone = r.Phone,
				City = r.City,
				Region = r.Region,
				Address = r.Address,
				
			}).Where(h => h.HotelId == id); ;
		}
		[HttpPost("{id}")]
		public object GetRoomType(int id)
		{
			return _context.Rooms.Include(h => h.Hotel).Include(rimg => rimg.RoomImages).Where(i => i.HotelId == id).Select(r => new
			{
				room = new
				{
					price = r.Price,
					type = r.Type,
					roomId = r.RoomId,
					hotelId = r.HotelId,
					maxGuests = r.MaxGuests,
				},
				pics=r.RoomImages.ToList(),
			}).ToList() ;


		}
	}
}
