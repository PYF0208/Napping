using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;

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
		[HttpGet("{num}")]
		public IEnumerable<ResultViewModel> Get(double num)
		{
			var result = _context.Hotels.Join(_context.Rooms, h => h.HotelId, r => r.HotelId, (h, r) => new ResultViewModel
			{
				HotelId = h.HotelId,
				Name = h.Name,
				Image = h.Image,
				City = h.City,
				Region = h.Region,
				Price = r.Price,
				MaxGuests = r.MaxGuests,
			}).OrderBy(hr => hr.Price);


			switch (num) {
				
				case >= 5:
					return result.Where(hr => hr.MaxGuests == 6);
				case >= 3:
					return result.Where(hr => hr.MaxGuests == 4);
				default:
					return result.Where(hr => hr.MaxGuests == 2);
				
			}


		}




	};
}
