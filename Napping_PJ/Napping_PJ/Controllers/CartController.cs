﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using Napping_PJ.Enums;
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
        /// <summary>
        /// 加入購物車
        /// </summary>
        /// <param name="roomDetailViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddSession([FromBody] RoomDetailViewModel roomDetailViewModel)
        {
            //確認客戶有選擇時間
            if (roomDetailViewModel == null || roomDetailViewModel.checkIn == null || roomDetailViewModel.checkOut == null)
            {
                return BadRequest("請選擇時間");
            }
            //將時間轉換成當地時間
            var roomCheckInTime = roomDetailViewModel.checkIn.ToLocalTime();
            var roomCheckOutTime = roomDetailViewModel.checkOut.ToLocalTime();
            //確定登入者
            Customer loginUser = await CheckLoginedUser();
            if (loginUser == null)
            {
                return BadRequest("/Login/Index");
            }
            //取得RoomDetailViewModel列表
            List<RoomDetailViewModel> roomDetailViewModels = GetSessionRoomDetails(loginUser);
            //確定房間是否已被訂
            //1確認OrderDetail
            Task<bool> isBookedTask = _context.OrderDetails.Include(x => x.Order).ThenInclude(x => x.Payments)
                .AnyAsync(x => x.RoomId == roomDetailViewModel.roomId
                               && x.CheckIn.Date >= DateTime.Today
                               && x.Order.Status < (int)PaymentStatusEnum.Cancel
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
                return Ok("房間已被下訂");
            }
            //重新計價
            (roomDetailViewModel.tRoomPrice, roomDetailViewModel.tServicePrice, roomDetailViewModel.tPromotionPrice) = await Recalculate(roomDetailViewModel, loginUser);
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
        /// <summary>
        /// 重新計價，防止客戶從前端修改資料送到後端
        /// </summary>
        /// <param name="roomDetailViewModel"></param>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        internal async Task<(double, double, double)> Recalculate(RoomDetailViewModel roomDetailViewModel, Customer loginUser)
        {

            var roomCheckInTime = roomDetailViewModel.checkIn.ToLocalTime();
            var roomCheckOutTime = roomDetailViewModel.checkOut.ToLocalTime();

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
            int days = (roomCheckOutTime.Date - roomCheckInTime.Date).Days;//確認下訂天數
            //尋找房型
            var room = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == roomDetailViewModel.roomId);
            var basePrice = room.Price; //找到該房型的基礎價格
            roomDetailViewModel.basePrice = basePrice;//修改roomDetailViewModel的basePrice房型價格，避免客戶從前端修改
            double tRoomPrice = 0;
            double tPromotPrice = 0;
            //計算每日的房型售價與房型折扣
            for (int i = 0; i < days; i++)
            {
                //找日期成數//重計房型日價
                Profit profit = await _context.Profits.FirstOrDefaultAsync(x => x.Date == roomCheckInTime.AddDays(i).Date);
                double profitNumber = profit == null ? 1 : profit.Number;
                tRoomPrice += basePrice * profitNumber;
                //找當日折扣金額
                var promots = _context.Promotions
                    .Where(x => x.EndDate >= DateTime.Today && x.LevelId == loginUser.LevelId
                                                            && (x.StartDate <= roomCheckInTime.AddDays(i) && roomCheckInTime.AddDays(i) <= x.EndDate));
                tPromotPrice += promots.Sum(x => x.Discount);
            }
            return (tRoomPrice, tServicePrice, tPromotPrice);
        }
        /// <summary>
        /// 刪除相應的購物車項目
        /// </summary>
        /// <param name="roomDetailViewModel"></param>
        /// <returns></returns>
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
            List<RoomDetailViewModel> roomDetailViewModels = GetSessionRoomDetails(loginUser);
            //找到欲刪除項目
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
        /// <summary>
        /// 確認使用者已登入
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 取得該使用者加入購物車的房型內容
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        internal List<RoomDetailViewModel> GetSessionRoomDetails(Customer loginUser)
        {
            List<RoomDetailViewModel> roomDetailViewModels = new List<RoomDetailViewModel>();
            //取得session內的購物車資訊
            byte[] cartByte = HttpContext.Session.Get($"{loginUser.CustomerId}_cartItem");
            //如session內有值
            if (cartByte != null)
            {
                string cartJson = System.Text.Encoding.UTF8.GetString(cartByte);//轉UTF8
                roomDetailViewModels = JsonConvert.DeserializeObject<List<RoomDetailViewModel>>(cartJson);//反序列成List<RoomDetailViewModel>
            }
            return roomDetailViewModels;
        }
        /// <summary>
        /// 取得客戶的購物車資訊
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetSession()
        {
            Claim userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (userEmailClaim != null)

            {
                Customer loginUser = await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmailClaim.Value);

                //byte[] bytes = HttpContext.Session.Get($"{loginUser.CustomerId}_cartItem");
                //if (bytes == null)
                //{
                //    return BadRequest("購物車bytes為空");
                //}
                //string json = System.Text.Encoding.UTF8.GetString(bytes);
                //IEnumerable<RoomDetailViewModel> roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
                IEnumerable<RoomDetailViewModel> roomDetailViewModels = GetSessionRoomDetails(loginUser);
                return Ok(roomDetailViewModels);
            }

            return BadRequest("尚未登入");
        }
        /// <summary>
        /// 修改相應的購物車內容
        /// </summary>
        /// <param name="roomDetailViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> SetSession([FromBody] RoomDetailViewModel roomDetailViewModel)
        {
            //確定登入者
            Customer loginUser = await CheckLoginedUser();
            if (loginUser == null)
            {
                return BadRequest("/Login/Index");
            }
            //取得RoomDetailViewModel列表
            List<RoomDetailViewModel> roomDetailViewModels = GetSessionRoomDetails(loginUser);
            //找到欲修改項目

            var findRDVM = roomDetailViewModels.Find(x => x.SessionId == roomDetailViewModel.SessionId);
            if (findRDVM == null)
            {
                return BadRequest("無此項");
            }


            (roomDetailViewModel.tRoomPrice, roomDetailViewModel.tServicePrice, roomDetailViewModel.tPromotionPrice) = await Recalculate(roomDetailViewModel, loginUser);

            // 获取roomDetailViewModel的所有属性
            var properties = roomDetailViewModel.GetType().GetProperties();

            // 循环遍历每个属性并将其值赋给findRDVM的相应属性
            foreach (var property in properties)
            {
                // 获取roomDetailViewModel属性的值
                var value = property.GetValue(roomDetailViewModel);

                // 设置findRDVM的相应属性的值
                property.SetValue(findRDVM, value);
            }

            //存入session列表
            byte[] bytes = null;
            string json = JsonConvert.SerializeObject(roomDetailViewModels);
            bytes = System.Text.Encoding.UTF8.GetBytes(json);
            HttpContext.Session.Set($"{loginUser.CustomerId}_cartItem", bytes);
            return Ok();

        }
    }
}
