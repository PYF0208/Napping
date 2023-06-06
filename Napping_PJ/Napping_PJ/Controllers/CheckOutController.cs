using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Enums;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using Napping_PJ.Utility;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace Napping_PJ.Controllers
{
    public class CheckOutController : Controller
    {
        public db_a989f8_nappingContext _Context { get; private set; }
        public CheckOutController(db_a989f8_nappingContext context)
        {
            _Context = context;
        }
        //public async Task<IActionResult> Index()
        //{
        //	string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        //	Customer loginedUsr = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmail);
        //	byte[] cartBytes = HttpContext.Session.Get($"{loginedUsr.CustomerId}_cartItem");
        //	if (cartBytes == null)
        //	{
        //		return BadRequest("購物車為空");
        //	}
        //	string json = System.Text.Encoding.UTF8.GetString(cartBytes);
        //	IEnumerable<RoomDetailViewModel> roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
        //	return View(roomDetailViewModels);
        //}
        public async Task<IActionResult> Index()
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = await GetCheckOutListBySession();
            return View(roomDetailViewModels);
        }
        public async Task<IActionResult> IndexByOrder(int orderId)
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = await GetCheckOutListByOrderId(orderId);
            return View("Index", roomDetailViewModels);
        }
        public async Task<IEnumerable<RoomDetailViewModel>> GetCheckOutListBySession()
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = new List<RoomDetailViewModel>();
            string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            Customer loginedUsr = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmail);
            byte[] cartBytes = HttpContext.Session.Get($"{loginedUsr.CustomerId}_cartItem");
            if (cartBytes != null)
            {
                string json = System.Text.Encoding.UTF8.GetString(cartBytes);
                roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
            }
            return roomDetailViewModels;
        }

        public async Task<IEnumerable<RoomDetailViewModel>> GetCheckOutListByOrderId(int orderId)
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = new List<RoomDetailViewModel>();
            Claim userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return roomDetailViewModels;
            }
            string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            Customer loginedUsr = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmail);
            Order checkOutOrder = await _Context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId && x.CustomerId == loginedUsr.CustomerId);
            if (checkOutOrder == null)
            {
                return roomDetailViewModels;
            }

            if (checkOutOrder.Status > 1)
            {
                return roomDetailViewModels;
            }

            roomDetailViewModels = _Context.OrderDetails.Include(x => x.Room).Include(x => x.Room.Hotel).Include(x => x.Room.RoomImages).Include(x => x.Room.Hotel.HotelExtraServices).ThenInclude(x => x.ExtraService).Where(x => x.Order == checkOutOrder).Include(x => x.OrderDetailExtraServices).Select(x =>

                new RoomDetailViewModel
                {
                    SessionId = 0,
                    roomId = checkOutOrder.OrderId,
                    roomType = x.Room.Type,
                    hotelName = x.Room.Hotel.Name,
                    roomImages = x.Room.RoomImages.Select(x => new roomImagesViewModel() { image = x.Image, }).ToList(),
                    availableCheckInTime = 16,
                    latestCheckOutTime = 12,
                    checkIn = x.CheckIn,
                    checkOut = x.CheckOut,
                    maxGuests = x.NumberOfGuests,
                    travelType = x.TravelType,
                    basePrice = x.Room.Price,
                    note = x.Note,
                    tRoomPrice = x.RoomTotalPrice,
                    tServicePrice = x.EspriceTotal,
                    tPromotionPrice = x.DiscountTotalPrice,
                    roomFeatures = x.Room.Features.Select(x => new roomFeatureViewModel()
                    {
                        featureId = x.FeatureId,
                        image = x.Image,
                        name = x.Name
                    }).ToList(),
                    selectedExtraServices = x.OrderDetailExtraServices.Select(y => new selectedExtraServiceViewModel
                    {
                        extraServiceId = -1,
                        name = y.ExtraServiceName,
                        serviceImage = "",
                        servicePrice = y.SingleServicePrice,
                        serviceQuantity = y.Number,
                    }).ToList(),
                    profitDictionary = null
                }
            );
            return roomDetailViewModels;
        }
        /// <summary>
        /// 支付完成返回網址
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CheckOutReturn()
        {
            // 接收參數
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            // 解密訊息
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼

            string TradeInfoDecrypt = CryptoUtil.DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
            NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
            //將Order付款狀態碼改成2
            ViewBag.OrderInfo = new
            {
                orderId = decryptTradeCollection["MerchantOrderNo"],
                totalPrice = decryptTradeCollection["Amt"]
            };
            if (decryptTradeCollection["Status"] == "SUCCESS")
            {
                Payment newPayment = new Payment()
                {
                    Date = DateTime.Now,
                    OrderId = Int32.Parse(decryptTradeCollection["MerchantOrderNo"]),
                    Status = (int)PaymentStatusEnum.Paid,
                    Type = decryptTradeCollection["PaymentType"]
                };
                try
                {
                    Order getOrder = await _Context.Orders.FirstOrDefaultAsync(x => x.OrderId == Int32.Parse(decryptTradeCollection["MerchantOrderNo"]));
                    getOrder.Status = (int)PaymentStatusEnum.Paid;
                    _Context.Payments.Add(newPayment);
                    await _Context.SaveChangesAsync();
                    return View("Success");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            receive.Length = 0;
            foreach (String key in decryptTradeCollection.AllKeys)
            {
                receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            }
            ViewData["TradeInfo"] = receive.ToString();

            return View("Error");
            //return View();
        }
    }
}
