using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using NuGet.Packaging;

namespace Napping_PJ.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class RoomsController : Controller
	{
		private readonly db_a989f8_nappingContext _context;

		public RoomsController(db_a989f8_nappingContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		// GET: Admin/Rooms
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
		[HttpPost]
		public async Task<string> CreateFeature([FromBody] FeatureViewModel feature)
		{
			Feature NewFeature = new Feature
			{
				FeatureId = feature.FeatureId,
				Name = feature.Name,
				Image = feature.Image,
			};
			_context.Features.Add(NewFeature);
			await _context.SaveChangesAsync();
			return "新增成功";
		}
		[HttpGet]
		public async Task<IEnumerable<FeatureViewModel>> GetFeature()
		{
			var Feature = await _context.Features
				.Select(Feature => new FeatureViewModel
				{
					FeatureId = Feature.FeatureId,
					Name = Feature.Name,
					Image = Feature.Image
				})
				.ToListAsync();
			return Feature;
		}
		[HttpPut]
		public async Task<string> Feature(int id, [FromBody] FeatureViewModel feature)
		{
			if (id != feature.FeatureId)
			{
				return "修改失敗！";
			}
			Feature ft = await _context.Features.FindAsync(feature.FeatureId);
			ft.FeatureId = feature.FeatureId;
			ft.Name = feature.Name;
			ft.Image = feature.Image;
			_context.Entry(ft).State = EntityState.Modified;
			try
			{
				_context.Update(ft);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FeatureExists(id))
				{
					return "修改失敗！";
				}
				else
				{
					throw;
				}
			}
			return "修改成功！";
		}
		[HttpDelete]
		public async Task<string> DeleteFeature(int id)
		{
			var feature = await _context.Features.FindAsync(id);
			if (feature == null)
			{
				return "刪除特徵成功!";
			}
			_context.Features.Remove(feature);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				return "刪除特徵記錄失敗!";
			}
			return "刪除特徵記錄成功!";
		}
		[HttpPost]
		public async Task<IEnumerable<RoomsViewModel>> GetRooms(RoomsViewModel rmViewMod)
		{
			var Rooms = await _context.Rooms.Include(p => p.Hotel).Include(x => x.Features).Include(q => q.RoomImages)
			.Select(Rooms => new RoomsViewModel
			{
				RoomId = Rooms.RoomId,
				HotelId = Rooms.HotelId,
				Name = Rooms.Hotel.Name,
				Type = Rooms.Type,
				Price = Rooms.Price,
				MaxGuests = Rooms.MaxGuests,
				Pic = Rooms.RoomImages.Select(y => new ImageInRoomViewModel
				{
					Image = y.Image,
				}).ToList(),
				Feature = Rooms.Features.Select(x => new FeatureInRoomViewModel
				{
					Id = x.FeatureId,
					Name = x.Name,
					Image = x.Image
				}).ToList(),
			}).ToListAsync();
			return Rooms;
		}
		// GET: Admin/Rooms/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Rooms == null)
			{
				return NotFound();
			}
			var room = await _context.Rooms
				.Include(r => r.Hotel)
				.FirstOrDefaultAsync(m => m.RoomId == id);
			if (room == null)
			{
				return NotFound();
			}

			return View(room);
		}
		// GET: Admin/Rooms/Create
		public IActionResult Create()
		{
			ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "name");
			return View();
		}
		// POST: Admin/Rooms/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		public async Task<string> CreateRooms([FromBody] CreateViewModel room)
		{
			Room NewRoom = new Room
			{
				HotelId = room.HotelId,
				Type = room.Type,
				Price = room.Price,
				MaxGuests = room.MaxGuests,
			};
			var images = room.Pic.Select(x => new RoomImage
			{
				Image = x
			});
			NewRoom.RoomImages.AddRange(images);
			_context.Rooms.Add(NewRoom);
			await _context.SaveChangesAsync();
			return "新增成功";
		}
		// GET: Admin/Rooms/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Rooms == null)
			{
				return NotFound();
			}
			var room = await _context.Rooms.FindAsync(id);
			if (room == null)
			{
				return NotFound();
			}
			ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "name", room.HotelId);
			return View(room);
		}
		// POST: Admin/Rooms/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		public string Edit(int id, [FromBody] RoomsEditViewModel room)
		{
			if (id == null || room.RoomId == null || id != room.RoomId)
			{
				return "修改失敗1";
			}
			var SearchHotel = _context.Rooms.Include(x => x.Features).SingleOrDefault(x => x.RoomId == id);
			if (SearchHotel == null)
			{
				return "修改失敗2";
			}
			SearchHotel.RoomId = room.RoomId;
			SearchHotel.HotelId = room.HotelId;
			SearchHotel.Type = room.Type;
			SearchHotel.Price = room.Price;
			SearchHotel.MaxGuests = room.MaxGuests;
			SearchHotel.Features.Clear();
			var x = _context.Features.Where(x => room.Features.Contains(x.FeatureId)).ToList();
			SearchHotel.Features.AddRange(x);
			try
			{
				_context.Update(SearchHotel);
				_context.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!RoomExists(room.RoomId))
				{
					return "修改失敗3";
				}
				else
				{
					throw;
				}
			}
			return "修改成功";

		}
		// GET: Admin/Rooms/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Rooms == null)
			{
				return NotFound();
			}

			var room = await _context.Rooms
				.Include(r => r.Hotel)
				.FirstOrDefaultAsync(m => m.RoomId == id);
			if (room == null)
			{
				return NotFound();
			}

			return View(room);
		}
		// POST: Admin/Rooms/Delete/5
		[HttpDelete]
		public async Task<string> DeleteConfirmed(int id)
		{
			var delete = await _context.Rooms.FindAsync(id);
			if (delete == null)
			{
				return "刪除失敗1";
			}
			var pics = _context.RoomImages.Where(r => r.RoomId == id);
			_context.Rooms.Remove(delete);
			_context.RoomImages.RemoveRange(pics);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				return "刪除失敗2";
			}
			return "刪除成功";
		}
		private bool RoomExists(int id)
		{
			return (_context.Rooms?.Any(e => e.RoomId == id)).GetValueOrDefault();
		}
		private bool FeatureExists(int id)
		{
			return (_context.Features?.Any(e => e.FeatureId == id)).GetValueOrDefault();
		}
	}
}
