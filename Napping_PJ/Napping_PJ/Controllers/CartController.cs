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

        [HttpPost]
        public async Task<IActionResult> AddSession([FromBody] RoomDetailViewModel roomDetailViewModel)
        {
	        if (roomDetailViewModel == null || roomDetailViewModel.checkIn == null || roomDetailViewModel.checkOut == null)
	        {
		        return BadRequest("請選擇時間");
	        }
            var roomCheckInTime = roomDetailViewModel.checkIn.ToLocalTime();
            var roomCheckOutTime = roomDetailViewModel.checkOut.ToLocalTime();
            //確定登入者
            Customer loginUser = await CheckLoginedUser();
            if (loginUser == null)
            {
                return BadRequest("/Login/Index");
            }
            //取得RoomDetailViewModel列表
            List<RoomDetailViewModel> roomDetailViewModels = GetRoomDetails(loginUser);
            //確定房間是否已被訂
            //1確認OrderDetail
            Task<bool> isBookedTask = _context.OrderDetails.Include(x => x.Order).ThenInclude(x => x.Payments)
                .AnyAsync(x => x.RoomId == roomDetailViewModel.roomId
                               && x.CheckIn.Date >= DateTime.Today
                               && x.Order.Payments.OrderBy(x => x.PaymentId).Last().Status < 3
                               && ((roomCheckInTime >= x.CheckIn && roomCheckInTime <= x.CheckIn)
                               || (roomCheckOutTime >= x.CheckOut && roomCheckOutTime <= x.CheckOut)
                               || (roomCheckInTime <= x.CheckOut && roomCheckOutTime >= x.CheckOut)));
            //2確認Session
            bool isInCart = roomDetailViewModels.Any(x =>
                x.roomId == roomDetailViewModel.roomId
                && ((roomCheckInTime >= x.checkIn && roomCheckInTime <= x.checkIn)
                || (roomCheckOutTime >= x.checkOut && roomCheckOutTime <= x.checkOut)
                || (roomCheckInTime <= x.checkOut && roomCheckOutTime >= x.checkOut)));

            await Task.WhenAll(isBookedTask); // 等待isBooked完成

            bool isBooked = isBookedTask.Result; // 获取isBooked的结果

            if (isInCart || isBooked)
            {
                return Ok("房間已下訂");
            }
            //重計服務價格
            double tServicePrice = 0;
            roomDetailViewModel.selectedExtraServices.ForEach(ex =>
             {
                 ex.servicePrice = _context.HotelExtraServices.FirstOrDefault(x => x.ExtraServiceId == ex.extraServiceId).ExtraServicePrice;
             });
            tServicePrice = roomDetailViewModel.selectedExtraServices.Sum(x => x.servicePrice * x.serviceQuantity);
            //重計房間價格
            //var checkInTime = roomDetailViewModel.checkIn;
            //var checkOutTime = roomDetailViewModel.checkOut;
            int days = (roomCheckOutTime.Date - roomCheckInTime.Date).Days;
            var room = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == roomDetailViewModel.roomId);
            var basePrice = room.Price;
            roomDetailViewModel.basePrice = basePrice;
            double tRoomPrice = 0;
            double tPromotPrice = 0;
            for (int i = 0; i < days; i++)
            {
                //找日期成數//重計折扣
                Profit profit = await _context.Profits.FirstOrDefaultAsync(x => x.Date == roomCheckInTime.AddDays(i).Date);
                double profitNumber = profit == null ? 1 : profit.Number;
                tRoomPrice += basePrice * profitNumber;
                //找折扣金額
                var promots = _context.Promotions
                    .Where(x => x.StartDate >= DateTime.Today && x.LevelId == loginUser.LevelId
                     && (x.StartDate <= roomCheckInTime.AddDays(i) && roomCheckInTime.AddDays(i) <= x.EndDate));
                tPromotPrice += promots.Sum(x => x.Discount);
            }
            roomDetailViewModel.tRoomPrice = tRoomPrice;
            roomDetailViewModel.tServicePrice = tServicePrice;
            roomDetailViewModel.tPromotionPrice = tPromotPrice;
            //寫入列表
            roomDetailViewModel.SessionId = 0;
            if (roomDetailViewModels.Count() != 0)
            {
                int maxSessionId = roomDetailViewModels.Max(x => x.SessionId);
                roomDetailViewModel.SessionId = ++maxSessionId;
            }
            roomDetailViewModels.Add(roomDetailViewModel);
            //存入session列表
            byte[] bytes = null;
            string json = JsonConvert.SerializeObject(roomDetailViewModels);
            bytes = System.Text.Encoding.UTF8.GetBytes(json);
            HttpContext.Session.Set($"{loginUser.CustomerId}_cartItem", bytes);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveSession([FromBody] RoomDetailViewModel roomDetailViewModel)
        {
            //確定登入者
            Customer loginUser = await CheckLoginedUser();
            if (loginUser == null)
            {
                return BadRequest("/Login/Index");
            }
            //取得RoomDetailViewModel列表
            List<RoomDetailViewModel> roomDetailViewModels = GetRoomDetails(loginUser);
            //刪除
            var getRDVM = roomDetailViewModels.FirstOrDefault(x => x.SessionId == roomDetailViewModel.SessionId);
            if (getRDVM == null)
            {
                return BadRequest("無此項");
            }
            roomDetailViewModels.Remove(getRDVM);
            //存入session列表
            byte[] bytes = null;
            string json = JsonConvert.SerializeObject(roomDetailViewModels);
            bytes = System.Text.Encoding.UTF8.GetBytes(json);
            HttpContext.Session.Set($"{loginUser.CustomerId}_cartItem", bytes);
            return Ok();
        }
        internal async Task<Customer> CheckLoginedUser()
        {
            Customer loginCustomer = null;
            Claim userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (userEmailClaim != null)
            {
                loginCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.Email == userEmailClaim.Value);
            }
            return loginCustomer;
        }
        internal List<RoomDetailViewModel> GetRoomDetails(Customer loginUser)
        {
            List<RoomDetailViewModel> roomDetailViewModels = new List<RoomDetailViewModel>();
            byte[] cartByte = HttpContext.Session.Get($"{loginUser.CustomerId}_cartItem");
            if (cartByte != null)
            {
                string cartJson = System.Text.Encoding.UTF8.GetString(cartByte);
                roomDetailViewModels = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(cartJson);
            }
            return roomDetailViewModels;
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
                    return BadRequest("購物車bytes為空");
                }
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                IEnumerable<RoomDetailViewModel> roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
                return Ok(roomDetailViewModels);
            }

            return BadRequest("尚未登入");
        }
    }
}
