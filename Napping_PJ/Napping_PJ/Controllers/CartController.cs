using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Runtime.ConstrainedExecution;

namespace Napping_PJ.Controllers
{
    public class CartController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public CartController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetRooms()
        {
            List<RoomDetailViewModel> cartViewModels = new List<RoomDetailViewModel>();
            List<Room> rooms = await _context.Rooms.Include(r => r.Hotel).Include(r => r.RoomImages).Take(5).ToListAsync();

            foreach (Room room in rooms)
            {
                cartViewModels.Add(new RoomDetailViewModel()
                {
                    RoomId = room.RoomId,
                    RoomType = room.Type,
                    HotelName = room.Hotel.Name,
                    RoomImages = room.RoomImages,
                    CheckIn = DateTime.Now,
                    CheckOut = DateTime.Now,
                    MaxGuests = room.MaxGuests,
                    TravelType = 0,
                    Note = null,
                    SelectedExtraServices = await GetExtraServices(room.HotelId)
                });
            }
            return Ok(cartViewModels);
        }
        public async Task<List<SelectedExtraServiceViewModel>> GetExtraServices(int HotelId)
        {
            List<SelectedExtraServiceViewModel> selectedExtraServiceViewModels = new List<SelectedExtraServiceViewModel>();
            IQueryable<ExtraService> extraServices = _context.ExtraServices.Where(es => es.HotelId == HotelId);
            List<ExtraService> extraServiceList = await extraServices.ToListAsync();

            foreach (ExtraService extraService in extraServiceList)
            {
                selectedExtraServiceViewModels.Add(
                    new SelectedExtraServiceViewModel()
                    {
                        ExtraServiceId = extraService.HotelId,
                        Name = extraService.Name,
                        ServiceQuantity = 0
                    });
            };

            return selectedExtraServiceViewModels;
        }
    }
}
