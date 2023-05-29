using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models.Entity;
using System.Text.Json.Serialization;
using System.Text.Json;
using Napping_PJ.Models;

namespace Napping_PJ.Controllers
{
    public class RoomDetailController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public RoomDetailController(db_a989f8_nappingContext context)
        {
            _context = context;
        }
        [Route("RoomDetail/{roomId}")]
        public IActionResult Index(int roomId)
        {
            ViewBag.RoomId = roomId;
            return View();
        }
        [Route("RoomDetail/GetImageList/{roomId}")]
        public IActionResult GetImageList(int roomId)
        {
            Room getRoom = _context.Rooms.Include(x => x.RoomImages).FirstOrDefault(x => x.RoomId == roomId);
            if (getRoom == null)
            {
                return BadRequest("無此Room");
            }
            //設置阻止循環引用
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return Ok(JsonSerializer.Serialize(getRoom.RoomImages, options));
        }
        [Route("RoomDetail/GetRoomDetail/{roomId}")]
        public async Task<IActionResult> GetRoomDetail(int roomId)
        {
            Room getRoom = await _context.Rooms.Include(x => x.Hotel).ThenInclude(x => x.ExtraServices).Include(x =>x.RoomImages).FirstOrDefaultAsync(x => x.RoomId == roomId);
            if (getRoom == null)
            {
                return BadRequest("無此房型");
            }
            CartViewModel CVM = new CartViewModel()
            {
                RoomId = getRoom.RoomId,
                RoomType = getRoom.Type,
                HotelName = getRoom.Hotel.Name,
                RoomImages = getRoom.RoomImages,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                MaxGuests = getRoom.MaxGuests,
                TravelType = 0,
                Note = null,
                //SelectedExtraServices = await GetExtraServices(getRoom.HotelId)
                SelectedExtraServices = getRoom.Hotel.ExtraServices.Select(x =>
                {
                    return new SelectedExtraServiceViewModel()
                    {
                        ExtraServiceId = x.ExtraServiceId,
                        Name = x.Name,
                        ServiceQuantity = 0,
                    };
                })
            };

            //設置阻止循環引用
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            return Ok(JsonSerializer.Serialize(CVM,options));
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
        public IActionResult GetBookingState(int roomId)
        {
            return Ok();
        }
    }
}
