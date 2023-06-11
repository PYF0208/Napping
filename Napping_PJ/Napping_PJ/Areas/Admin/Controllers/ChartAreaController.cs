using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ChartAreaController : Controller
	{
		private readonly db_a989f8_nappingContext _context;

		public ChartAreaController(db_a989f8_nappingContext context ){

			_context = context;

		}

		public IEnumerable<ChartAreaViewModel> GetOrderData()
		{
			return _context.Orders.Join(_context.OrderDetails, o => o.OrderId , od => od.OrderId, (o , od) => new ChartAreaViewModel
			{
				OrderId = o.OrderId,
				Date = o.Date,
				TotalPrice = od.RoomTotalPrice + od.EspriceTotal - od.DiscountTotalPrice,

			}).OrderBy(o => o.Date);


		}

		public IEnumerable<ChartBarViewModel> GetHotelData() {

			return _context.Hotels.GroupBy(x => x.City).Select(c => new ChartBarViewModel { 
			
			City = c.Key,
			Count = c.Count()
			
			});
		
		
		}
	}
}
