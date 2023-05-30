using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models.Entity;
using Napping_PJ.Models;
using Newtonsoft.Json;

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
        [Route("RoomDetail/GetRoomDetail/{roomId}")]
        public async Task<IActionResult> GetRoomDetail(int roomId)
        {
            Room getRoom = await _context.Rooms.Include(x => x.Hotel).ThenInclude(x => x.ExtraServices).Include(x => x.RoomImages).Include(x => x.Features).FirstOrDefaultAsync(x => x.RoomId == roomId);
            if (getRoom == null)
            {
                return BadRequest("無此房型");
            }
            RoomDetailViewModel CVM = new RoomDetailViewModel()
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
                RoomFeatures = getRoom.Features.Select(x =>
                {
                    return new RoomFeatureViewModel()
                    {
                        FeatureId = x.FeatureId,
                        Name = x.Name,
                        Image = x.Image,
                    };
                }),
                //SelectedExtraServices = await GetExtraServices(getRoom.HotelId)
                SelectedExtraServices = getRoom.Hotel.ExtraServices.Select(x =>
                {
                    return new SelectedExtraServiceViewModel()
                    {
                        ExtraServiceId = x.ExtraServiceId,
                        Name = x.Name,
                        ServiceQuantity = 0,
                    };
                }).ToList()
            };

            //設置阻止循環引用
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return Ok(JsonConvert.SerializeObject(CVM, settings));
        }
        [Route("RoomDetail/GetBookingState/{roomId}")]
        public async Task<IActionResult> GetBookingState(int roomId)
        {
            List<DateTime> bookedDate = new List<DateTime>();
            await _context.OrderDetails.Where(x => x.RoomId == roomId && (x.CheckIn >= DateTime.Today && x.CheckOut >= DateTime.Today)).ForEachAsync(x =>
            {
                var startDay = x.CheckIn.Date;
                var endDay = x.CheckOut.Date;
                var currentDay = startDay;
                while (currentDay <= endDay)
                {
                    bookedDate.Add(currentDay.Date);
                    currentDay = currentDay.AddDays(1);
                }
            });
            return Ok(bookedDate);
        }
    }
}
