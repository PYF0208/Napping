using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models.Entity;
using NuGet.Common;

namespace Napping_PJ.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HotelsController : Controller
	{
		private readonly db_a989f8_nappingContext _context;

		public HotelsController(db_a989f8_nappingContext context)
		{
			_context = context;
		}

		// GET: Admin/Hotels
		public async Task<IActionResult> Index()
		{
			return View();
		}
		[HttpGet]

		public async Task<IEnumerable<HotelsViewModel>> GetHotelsService()
		{
			var HotelsService = _context.Hotels.Select(HotelsService => new HotelsViewModel
			{
				HotelId = HotelsService.HotelId,
				Name = HotelsService.Name,
				Star = HotelsService.Star,
				Image = HotelsService.Image,
				ContactName = HotelsService.ContactName,
				Phone = HotelsService.Phone,
				Email = HotelsService.Email,
				City = HotelsService.City,
				Region = HotelsService.Region,
				Address = HotelsService.Address,
				AvgComment = HotelsService.AvgComment
			});
			return HotelsService;
		}
		[HttpGet]
		// GET: Admin/Hotels/Details/5
		public async Task<HotelsViewModel> Details(int? id, HotelsViewModel hotel)
		{
			var SearchHotel = await _context.Hotels
		.FirstOrDefaultAsync(h => h.HotelId == hotel.HotelId);

			if (SearchHotel == null)
			{
				return null;
			}
			var HotelsService = new HotelsViewModel
			{
				HotelId = hotel.HotelId,
				Name = hotel.Name,
				Star = hotel.Star,
				Image = hotel.Image,
				ContactName = hotel.ContactName,
				Phone = hotel.Phone,
				Email = hotel.Email,
				City = hotel.City,
				Region = hotel.Region,
				Address = hotel.Address,
				AvgComment = hotel.AvgComment
			};
			return HotelsService;
		}

		[HttpPost]
		public async Task<IEnumerable<HotelsViewModel>> FilterExtraServices(
			[FromBody] HotelsViewModel hotel)
		{
			return _context.ExtraServices.Where(h =>
			h.HotelId == hotel.HotelId ||
			h.Hotel.Name.Contains(hotel.Name)||
			h.Hotel.Star.Contains(hotel.Star)||
			h.Hotel.Image.Contains(hotel.Image)||
			h.Hotel.ContactName.Contains(hotel.ContactName)||
			h.Hotel.Phone.Contains(hotel.Phone)||
			h.Hotel.Email.Contains(hotel.Email)||
			h.Hotel.City.Contains(hotel.City)||
			h.Hotel.Region.Contains(hotel.Region)||
			h.Hotel.Address.Contains(hotel.Address)).Select(h => new HotelsViewModel
			{
				HotelId = hotel.HotelId,
				Name = hotel.Name,
				Star = hotel.Star,
				Image = hotel.Image,
				ContactName = hotel.ContactName,
				Phone = hotel.Phone,
				Email = hotel.Email,
				City = hotel.City,
				Region = hotel.Region,
				Address = hotel.Address,
				AvgComment = hotel.AvgComment
			});

		}


		// GET: Admin/Hotels/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Admin/Hotels/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		public async Task<string> NewCreate([FromBody] HotelsViewModel hotel)
		{

			Hotel NewHotel = new Hotel
			{
				HotelId = hotel.HotelId,
				Name = hotel.Name,
				Star = hotel.Star,
				Image = hotel.Image,
				ContactName = hotel.ContactName,
				Phone = hotel.Phone,
				Email = hotel.Email,
				City = hotel.City,
				Region = hotel.Region,
				Address = hotel.Address,
				AvgComment = hotel.AvgComment
			};
			_context.Hotels.Add(NewHotel);
			await _context.SaveChangesAsync();
			return "創建成功";
		}
		//else
		//{			
		//	return "創建失敗";
		//}	

		//var DuplicateAccount =await  _context.Hotels.FirstOrDefaultAsync(h =>  h.HotelId == hotel.HotelId );

		//if (DuplicateAccount == null)
		//{

		//}

		// GET: Admin/Hotels/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Hotels == null)
			{
				return NotFound();
			}

			var hotel = await _context.Hotels.FindAsync(id);
			if (hotel == null)
			{
				return NotFound();
			}
			return View();
		}

		// POST: Admin/Hotels/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPut]

		public async Task<string> Edit(int id, [FromBody] HotelsViewModel hotel)
		{
			if (id == null || hotel.HotelId == null || id != hotel.HotelId)
			{
				return "修改加購服務選項失敗!";
			}


			var SearchHotel = await _context.Hotels.FindAsync(id);
			if (SearchHotel == null)
			{
				return "修改加購服務選項失敗!";
			}
			SearchHotel.HotelId = hotel.HotelId;
			SearchHotel.Name = hotel.Name;
			SearchHotel.Star = hotel.Star;
			SearchHotel.Image = hotel.Image;
			SearchHotel.ContactName = hotel.ContactName;
			SearchHotel.Phone = hotel.Phone;
			SearchHotel.Email = hotel.Email;
			SearchHotel.City = hotel.City;
			SearchHotel.Region = hotel.Region;
			SearchHotel.Address = hotel.Address;
			SearchHotel.AvgComment = hotel.AvgComment;
			try
			{
				_context.Update(SearchHotel);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!HotelExists(hotel.HotelId))
				{
					return "修改加購服務選項失敗!";
				}
				else
				{
					throw;
				}
			}
			return "修改加購服務選項成功!";
		}





		// GET: Admin/Hotels/Delete/5

		public async Task<IActionResult> Delete(int? id)
		{


			if (id == null || _context.Hotels == null)
			{
				return null;
			}

			var hotel = await _context.Hotels
				.FirstOrDefaultAsync(m => m.HotelId == id);
			if (hotel == null)
			{
				return null;
			}

			return View();
		}

		// POST: Admin/Hotels/Delete/5
		[HttpDelete]

		public async Task<string> DeleteConfirmed(int id)
		{
			var del = await _context.Hotels.FindAsync(id);

			if (del == null)
			{
				return "無效";
			}
			//var SearchHotelId = await _context.Hotels.FindAsync(id);

			_context.Hotels.Remove(del);
			try
			{
				await _context.SaveChangesAsync();
				
			}
			catch (DbUpdateException ex)
			{
				return "刪除聯紀錄失敗";
			}

			return "刪除成功";



		}

		private bool HotelExists(int id)
		{
			return (_context.Hotels?.Any(e => e.HotelId == id)).GetValueOrDefault();
		}
	}
}
