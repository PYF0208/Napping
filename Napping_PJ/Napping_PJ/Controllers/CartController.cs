using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

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
        //public async Task<IActionResult> GetRooms()
        //{
        //    List<RoomDetailViewModel> cartViewModels = new List<RoomDetailViewModel>();
        //    List<Room> rooms = await _context.Rooms.Include(r => r.Hotel).Include(r => r.RoomImages).Take(5).ToListAsync();

        //    foreach (Room room in rooms)
        //    {
        //        cartViewModels.Add(new RoomDetailViewModel()
        //        {
        //            roomId = room.RoomId,
        //            roomType = room.Type,
        //            hotelName = room.Hotel.Name,
        //            roomImages = room.RoomImages.Select(x => new roomImagesViewModel()
        //            {
        //                image = x.Image,
        //            }).ToList(),
        //            checkIn = DateTime.Now,
        //            checkOut = DateTime.Now,
        //            maxGuests = room.MaxGuests,
        //            travelType = 0,
        //            note = null,
        //            totalPrice = 0,
        //            selectedExtraServices = await GetExtraServices(room.HotelId)
        //        });
        //    }
        //    return Ok(cartViewModels);
        //}
        //public async Task<List<selectedExtraServiceViewModel>> GetExtraServices(int HotelId)
        //{
        //    List<selectedExtraServiceViewModel> selectedExtraServiceViewModels = new List<selectedExtraServiceViewModel>();
        //    IQueryable<HotelExtraService> extraServices = _context.HotelExtraServices.Include(x => x.ExtraService).Where(es => es.HotelId == HotelId);
        //    List<HotelExtraService> extraServiceList = await extraServices.ToListAsync();

        //    foreach (HotelExtraService extraService in extraServiceList)
        //    {
        //        selectedExtraServiceViewModels.Add(
        //            new selectedExtraServiceViewModel()
        //            {
        //                extraServiceId = extraService.HotelId,
        //                name = extraService.ExtraService.Name,
        //                serviceQuantity = 0
        //            });
        //    };

        //    return selectedExtraServiceViewModels;
        //}
        public IActionResult CheckIsLogined()
        {
            Claim userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return BadRequest("/Login/Index");
            }

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> SetSession([FromBody] IEnumerable<RoomDetailViewModel> roomDetailViewModels)
        {
            Claim userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            Customer loginUser = await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmailClaim.Value);
            //string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            byte[] bytes = null;
            string json = JsonConvert.SerializeObject(roomDetailViewModels);
            bytes = System.Text.Encoding.UTF8.GetBytes(json);
            HttpContext.Session.Set($"{loginUser.CustomerId}_cartItem", bytes);
            return Ok();
        }

        public async Task<IActionResult> GetSession()
        {
            Claim userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (userEmailClaim != null)
            {
                Customer loginUser = await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmailClaim.Value);

                byte[] bytes = HttpContext.Session.Get($"{loginUser.CustomerId}_cartItem");
                if (bytes == null)
                {
                    return BadRequest("購物車為空");
                }
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                IEnumerable<RoomDetailViewModel> roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
                return Ok(roomDetailViewModels);
            }

            return BadRequest("尚未登入");
        }
    }
}
