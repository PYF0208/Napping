using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HotelExtraServicesController : Controller
    {
        
        private readonly db_a989f8_nappingContext _context;

        public HotelExtraServicesController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
		[HttpGet]
		public async Task<IEnumerable<HotelsViewModel>> GetHotelName()
		{
			var hotelsViewModel = await _context.Hotels
				.Select(h => new HotelsViewModel
				{
					HotelId = h.HotelId,
					Name = h.Name,
				})
				.ToListAsync();

			return hotelsViewModel;
		}

		[HttpGet]
		public async Task<IEnumerable<ExtraServiceViewModel>> GetExtraServiceName()
		{
			var extraServiceViewModel = await _context.ExtraServices
				.Select(es => new ExtraServiceViewModel
				{
					ExtraServiceId = es.ExtraServiceId,
					ExtraServiceName = es.Name,
					
				})
				.ToListAsync();

			return extraServiceViewModel;
		}

		[HttpPost]
		public async Task<IEnumerable<ExtraServiceViewModel>> FilterHotelExtraServices(
			[FromBody] ExtraServiceViewModel esViewModel)
		{
			return await _context.HotelExtraServices.Include(x=>x.Hotel).Include(x=>x.ExtraService).Where(es =>
			es.ExtraServicePrice == esViewModel.ExtraServicePrice ||
			es.Hotel.Name.Contains(esViewModel.HotelName) ||
			es.ExtraService.Name.Contains(esViewModel.ExtraServiceName)).Select(es => new ExtraServiceViewModel
			{
				HotelId = es.HotelId,
				ExtraServiceId = es.ExtraServiceId,
				ExtraServicePrice = es.ExtraServicePrice,
				HotelName=es.Hotel.Name,
				ExtraServiceName = es.ExtraService.Name,
				Image =es.ExtraService.Image,

			}).ToListAsync();

		}

		[HttpPut]
		public async Task<string> UpdateHotelExtraServices(int hotelId, int extraServiceId, [FromBody] ExtraServiceViewModel esViewModel)
		{
			if (hotelId != esViewModel.HotelId&& extraServiceId!=esViewModel.ExtraServiceId)
			{
				return "修改旅店所屬服務失敗！";
			}

			HotelExtraService he = await _context.HotelExtraServices.FindAsync(esViewModel.HotelId,esViewModel.ExtraServiceId);
			he.HotelId = esViewModel.HotelId;
			he.ExtraServiceId = esViewModel.ExtraServiceId;
			he.ExtraServicePrice = esViewModel.ExtraServicePrice;
			_context.Entry(he).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!HotelExtrServicesExists(hotelId,extraServiceId))
				{
					return "修改旅店所屬服務失敗！";
				}
				else
				{
					throw;
				}
			}

			return "修改旅店所屬服務成功！";
		}

		[HttpPost]
		public async Task<string> InsertHotelExtraServices([FromBody] ExtraServiceViewModel esViewModel)
		{
			HotelExtraService he = new HotelExtraService
			{
				HotelId = esViewModel.HotelId,
				ExtraServiceId = esViewModel.ExtraServiceId,
				ExtraServicePrice = esViewModel.ExtraServicePrice,

			};
			_context.HotelExtraServices.Add(he);
			await _context.SaveChangesAsync();

			return "新增旅店所屬服務成功！";
		}

		[HttpDelete]
		public async Task<string> DeleteHotelExtraServices(int hotelId, int extraServiceId)
		{

			var hotelExtraService = await _context.HotelExtraServices.FindAsync(hotelId,extraServiceId);
			if (hotelExtraService == null)
			{
				return "刪除旅店所屬服務失敗！";
			}

			_context.HotelExtraServices.Remove(hotelExtraService);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch
			{
				return "刪除關聯紀錄失敗！";
			}

			return "刪除旅店所屬服務成功！";
		}

		private bool HotelExtrServicesExists(int hotelId,int extraServiceId)
		{
			return (_context.HotelExtraServices?.Any(x => x.HotelId == hotelId&&x.ExtraServiceId== extraServiceId)).GetValueOrDefault();
		}
	}
}
