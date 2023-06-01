using System.Security.Claims;
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
            Room getRoom = await _context.Rooms.Include(x => x.Hotel).ThenInclude(x => x.HotelExtraServices).ThenInclude(x => x.ExtraService).Include(x => x.RoomImages).Include(x => x.Features).FirstOrDefaultAsync(x => x.RoomId == roomId);
            if (getRoom == null)
            {
                return BadRequest("/Home/Index");
            }
            RoomDetailViewModel CVM = new RoomDetailViewModel()
            {
                roomId = getRoom.RoomId,
                roomType = getRoom.Type,
                hotelName = getRoom.Hotel.Name,
                roomImages = getRoom.RoomImages.Select(x => new roomImagesViewModel()
                {
                    image = x.Image,
                }).ToList(),
                availableCheckInTime = 16,
                latestCheckOutTime = 12,
                checkIn = null,
                checkOut = null,
                maxGuests = getRoom.MaxGuests,
                travelType = 0,
                roomPrice = getRoom.Price,
                note = null,
                totalPrice = 0,
                roomFeatures = getRoom.Features.Select(x =>
                {
                    return new roomFeatureViewModel()
                    {
                        featureId = x.FeatureId,
                        name = x.Name,
                        image = x.Image,
                    };
                }),
                selectedExtraServices = getRoom.Hotel.HotelExtraServices.Select(x =>
                {
                    return new selectedExtraServiceViewModel()
                    {
                        extraServiceId = x.ExtraServiceId,
                        name = x.ExtraService.Name,
                        serviceImage = x.ExtraService.Image,
                        servicePrice = x.ExtraServicePrice,
                        serviceQuantity = 0,
                    };
                }).ToList(),
                profitDictionary = new Dictionary<long, double>()
            };
            await _context.Profits.Where(x => x.Date > DateTime.Today).ForEachAsync(x =>
            {
                CVM.profitDictionary.Add(new DateTimeOffset(x.Date).ToUnixTimeMilliseconds(), x.Number);
            });
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
            List<long[]> bookedDate = new List<long[]>();

            await _context.OrderDetails
                .Where(x => x.RoomId == roomId && (x.CheckIn >= DateTime.Today && x.CheckOut >= DateTime.Today))
                .ForEachAsync(
                    x =>
                    {
                        bookedDate.Add(new long[2]
                        {
                            new DateTimeOffset(x.CheckIn).ToUnixTimeMilliseconds(),
                            new DateTimeOffset(x.CheckOut).ToUnixTimeMilliseconds()
                        });
                    });
            //List<DateTime> bookedDate = new List<DateTime>();
            //await _context.OrderDetails.Where(x => x.RoomId == roomId && (x.CheckIn >= DateTime.Today && x.CheckOut >= DateTime.Today)).ForEachAsync(x =>
            //{
            //    var startDay = x.CheckIn;
            //    var endDay = x.CheckOut;
            //    var currentDay = startDay;
            //    while (currentDay <= endDay)
            //    {
            //        bookedDate.Add(currentDay);
            //        currentDay = currentDay.AddDays(1);
            //    }
            //});
            return Ok(bookedDate);
        }

        public async Task<IActionResult> GetPromotions()
        {
            Claim userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return BadRequest();
            }

            Customer loginedUser = await _context.Customers.FirstOrDefaultAsync(x => x.Email == userEmailClaim.Value);
            IQueryable<Promotion> promotions = _context.Promotions.Where(x => x.LevelId == loginedUser.LevelId);

        }
    }
}
