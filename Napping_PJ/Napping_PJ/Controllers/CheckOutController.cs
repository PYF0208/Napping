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
        /// <summary>
        /// 購物車轉結帳頁
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = await GetCheckOutListBySession();
            return View(roomDetailViewModels);
        }
        /// <summary>
        /// 重新結帳按鈕轉結帳頁
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<IActionResult> IndexByOrder(int orderId)
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = await GetCheckOutListByOrderId(orderId);
            return View("Index", roomDetailViewModels);
        }
        public async Task<IEnumerable<RoomDetailViewModel>> GetCheckOutListBySession()
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = new List<RoomDetailViewModel>();
            //從cookie取得當前登入者的email
            string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            //到資料庫找到當前登入的user
            Customer loginedUsr = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmail);
            //到session取得購物車資料
            byte[] cartBytes = HttpContext.Session.Get($"{loginedUsr.CustomerId}_cartItem");
            //如cartBytes不為null則將資料轉成RoomDetailViewModel陣列
            if (cartBytes != null)
            {
                string json = System.Text.Encoding.UTF8.GetString(cartBytes); //將 cartBytes 陣列轉換為 UTF-8 編碼的字串
                roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json); //將 json 字串反序列化為 IEnumerable<RoomDetailViewModel> 的集合
            }
            return roomDetailViewModels;
        }

        public async Task<IEnumerable<RoomDetailViewModel>> GetCheckOutListByOrderId(int orderId)
        {
            IEnumerable<RoomDetailViewModel> roomDetailViewModels = new List<RoomDetailViewModel>();
            //從cookie找到登入者的email claim
            Claim userEmailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return roomDetailViewModels;
            }
            //如claim存在，則取得email claim的值
            string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            //根據userEmail找到使用者
            Customer loginedUsr = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmail);
            //根據找到的使用者、與傳入的orderId，找到要結帳的訂單
            Order checkOutOrder = await _Context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId && x.CustomerId == loginedUsr.CustomerId);
            //如果沒找到
            if (checkOutOrder == null)
            {
                return roomDetailViewModels;
            }
            //如果訂單狀態不為NotPay(>1)
            if (checkOutOrder.Status > (int)PaymentStatusEnum.NotPay)
            {
                return roomDetailViewModels;
            }
            //將找到的訂單寫入roomDetailViewModels
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

            if (decryptTradeCollection["Status"] == "SUCCESS")
            {
                //取得OrderId
                string getOrderId = ((string)decryptTradeCollection["MerchantOrderNo"]).Split('_')[0];
                ViewBag.OrderInfo = new
                {
                    orderId = getOrderId,
                    totalPrice = decryptTradeCollection["Amt"]
                };
                //將Order付款狀態碼改成Paid(2)
                Payment newPayment = new Payment()
                {
                    Date = DateTime.Now,
                    OrderId = Int32.Parse(getOrderId),
                    Status = (int)PaymentStatusEnum.Paid,
                    Type = decryptTradeCollection["PaymentType"]
                };
                try
                {
                    //找到訂單
                    Order getOrder = await _Context.Orders.FirstOrDefaultAsync(x => x.OrderId == Int32.Parse(getOrderId));
                    getOrder.Status = (int)PaymentStatusEnum.Paid;//將訂單Status改成Paid
                    _Context.Payments.Add(newPayment);//新增payment紀錄
                    await _Context.SaveChangesAsync();
                    return View("Success");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return View("Error");

            #region 導到CheckOutReturn測試頁
            //receive.Length = 0;
            //foreach (String key in decryptTradeCollection.AllKeys)
            //{
            //    receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            //}
            //ViewData["TradeInfo"] = receive.ToString();
            //return View();
            #endregion
        }
    }
}
