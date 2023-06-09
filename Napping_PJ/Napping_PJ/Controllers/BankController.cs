using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Enums;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using Napping_PJ.Services;
using Napping_PJ.Utility;
using Newtonsoft.Json;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;

namespace Napping_PJ.Controllers
{
    public class BankController : Controller
    {
        private readonly db_a989f8_nappingContext _Context;
        private readonly IBackgroundJobClient backgroundJobs;
        private readonly IChangePaymentStatusService paymentStatusService;

        public BankController(db_a989f8_nappingContext context, IBackgroundJobClient backgroundJobs, IChangePaymentStatusService paymentStatusService)
        {
            _Context = context;
            this.backgroundJobs = backgroundJobs;
            this.paymentStatusService = paymentStatusService;
        }
        public void DelayPayment(int min)
        {


            //var jobId = BackgroundJob.Schedule(
            ////() => Console.WriteLine($"房型鎖定{min}分鐘!"),
            //() => bir.SendBirthDayMail(),
            //	TimeSpan.FromMinutes(min));
        }

        private BankInfoModel _bankInfoModel = new BankInfoModel
        {
            MerchantID = "MS149050874",
            HashKey = "yntmG4lRSvANsjKiwuP7r99pi13NKzsB",
            HashIV = "Cg6vxOtPYjevmcOP",
            ReturnURL = $"https://localhost:7265/CheckOut/CheckOutReturn",
            NotifyURL = "http://yourWebsitUrl/Bank/SpgatewayNotify",
            CustomerURL = "http://yourWebsitUrl/Bank/SpgatewayCustomer",
            AuthUrl = "https://ccore.newebpay.com/MPG/mpg_gateway",
            CloseUrl = "https://core.newebpay.com/API/CreditCard/Close"
        };


        [HttpPost]
        //[Route("Bank/SpgatewayPayBillAsync/")]
        public async Task SpgatewayPayBillAsync(string firstName = null, string phone = null, int orderId = -1)

        {
            Order thisOrder = new Order();
            int subTotle = 0;
            if (orderId < 0)
            {
                #region 寫入訂單

                var userEmialClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                if (userEmialClaim == null)
                {
                    return;
                }

                Customer loginUser = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmialClaim.Value);
                byte[] cartBytes = HttpContext.Session.Get($"{loginUser.CustomerId}_cartItem");
                if (cartBytes == null)
                {
                    return;
                }

                string json = System.Text.Encoding.UTF8.GetString(cartBytes);
                IEnumerable<RoomDetailViewModel> roomDetailViewModels =
                    JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
                if (roomDetailViewModels.Count() == 0)
                {
                    return;
                }

                var newOrder = new Order
                {
                    CustomerId = loginUser.CustomerId,
                    Date = DateTime.Now,
                    Payments = new List<Payment>(),
                    OrderDetails = new List<OrderDetail>(),
                    PhoneOfBooking = phone,
                    NameOfBooking = firstName,
                    Status = (int)PaymentStatusEnum.NotPay,
                    PaymentType = "信用卡"
                };

                newOrder.Payments.Add(new Payment()
                {
                    Order = newOrder,
                    Date = DateTime.Now,
                    Status = (int)PaymentStatusEnum.NotPay,
                    Type = "信用卡"
                });

                foreach (var rDVM in roomDetailViewModels)
                {
                    var newOrderDetail = new OrderDetail
                    {
                        RoomId = rDVM.roomId,
                        CheckIn = rDVM.checkIn,
                        CheckOut = rDVM.checkOut,
                        NumberOfGuests = rDVM.maxGuests,
                        TravelType = "who care",
                        RoomTotalPrice = rDVM.tRoomPrice,
                        DiscountTotalPrice = rDVM.tPromotionPrice,
                        EspriceTotal = rDVM.tServicePrice,
                        Note = rDVM.note,
                        Order = newOrder,
                    };

                    newOrderDetail.OrderDetailExtraServices = rDVM.selectedExtraServices.Where(x => x.serviceQuantity > 0)
                        .Select(x =>
                        {
                            return new OrderDetailExtraService
                            {
                                OrderDetailId = newOrderDetail.OrderDetailId,
                                ExtraServiceName = x.name,
                                Number = x.serviceQuantity,
                                OrderDetail = newOrderDetail,
                                SingleServicePrice = x.servicePrice
                            };
                        }).ToList();

                    newOrder.OrderDetails.Add(newOrderDetail);
                }

                _Context.Orders.Add(newOrder);
                await _Context.SaveChangesAsync();
                //清空購物車
                thisOrder = newOrder;
                subTotle = roomDetailViewModels.Sum(x => (Int32)(x.tPromotionPrice + x.tServicePrice + x.tRoomPrice));
                HttpContext.Session.Remove($"{loginUser.CustomerId}_cartItem");

                #endregion

                #region 寫入10分鐘鎖定任務

                //發送任務下單後10分鐘一到，自動確認訂單是否已付款，如未付款將state改為3
                var jobid = BackgroundJob.Schedule(() => paymentStatusService.CheckPaymentStatus(newOrder.OrderId),
                    TimeSpan.FromMinutes(10));

                #endregion

            }
            else
            {
                Order getOrder = await _Context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
                if (getOrder == null)
                {
                    return;
                }
                thisOrder = getOrder;
                subTotle = (int)_Context.OrderDetails.Where(x => x.OrderId == getOrder.OrderId)
                    .Sum(y => y.RoomTotalPrice + y.EspriceTotal - y.DiscountTotalPrice);
            }

            string version = "1.5";

            // 目前時間轉換 +08:00, 防止傳入時間或Server時間時區不同造成錯誤
            DateTimeOffset taipeiStandardTimeOffset = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0));

            TradeInfo tradeInfo = new TradeInfo()
            {
                // * 商店代號
                MerchantID = _bankInfoModel.MerchantID,
                // * 回傳格式
                RespondType = "String",
                // * TimeStamp
                TimeStamp = taipeiStandardTimeOffset.ToUnixTimeSeconds().ToString(),
                // * 串接程式版本
                Version = version,
                // * 商店訂單編號
                //MerchantOrderNo = thisOrder.OrderId.ToString(),
                MerchantOrderNo = $"{thisOrder.OrderId}_{DateTime.Now.ToString("yyyyMMddhhmmss")}",
                // * 訂單金額
                Amt = subTotle,
                // * 商品資訊
                ItemDesc = "預訂房間",
                // 繳費有效期限(適用於非即時交易)
                ExpireDate = null,
                // 支付完成 返回商店網址
                ReturnURL = _bankInfoModel.ReturnURL,
                // 支付通知網址
                NotifyURL = _bankInfoModel.NotifyURL,
                // 商店取號網址
                CustomerURL = _bankInfoModel.CustomerURL,
                // 支付取消 返回商店網址
                ClientBackURL = null,
                // * 付款人電子信箱
                Email = string.Empty,
                // 付款人電子信箱 是否開放修改(1=可修改 0=不可修改)
                EmailModify = 0,
                // 商店備註
                OrderComment = "Napping訂房網",
                // 信用卡 一次付清啟用(1=啟用、0或者未有此參數=不啟用)
                CREDIT = 1,
                // WEBATM啟用(1=啟用、0或者未有此參數，即代表不開啟)
                WEBATM = 1,
                // ATM 轉帳啟用(1=啟用、0或者未有此參數，即代表不開啟)
                VACC = 1,
                // 超商代碼繳費啟用(1=啟用、0或者未有此參數，即代表不開啟)(當該筆訂單金額小於 30 元或超過 2 萬元時，即使此參數設定為啟用，MPG 付款頁面仍不會顯示此支付方式選項。)
                CVS = 1,
                // 超商條碼繳費啟用(1=啟用、0或者未有此參數，即代表不開啟)(當該筆訂單金額小於 20 元或超過 4 萬元時，即使此參數設定為啟用，MPG 付款頁面仍不會顯示此支付方式選項。)
                BARCODE = null,

            };

            if (string.Equals("CREDIT", "CREDIT"))
            {
                tradeInfo.CREDIT = 1;
            }
            else if (string.Equals("WEBATM", "WEBATM"))
            {
                tradeInfo.WEBATM = 1;
            }
            else if (string.Equals("VACC", "VACC"))
            {
                // 設定繳費截止日期
                tradeInfo.ExpireDate = taipeiStandardTimeOffset.AddDays(1).ToString("yyyyMMdd");
                tradeInfo.VACC = 1;
            }
            else if (string.Equals("CVS", "CVS"))
            {
                // 設定繳費截止日期
                tradeInfo.ExpireDate = taipeiStandardTimeOffset.AddDays(1).ToString("yyyyMMdd");
                tradeInfo.CVS = 1;
            }
            else if (string.Equals("BARCODE", "BARCODE"))
            {
                // 設定繳費截止日期
                tradeInfo.ExpireDate = taipeiStandardTimeOffset.AddDays(1).ToString("yyyyMMdd");
                tradeInfo.BARCODE = 1;
            }

            Atom<string> result = new Atom<string>()
            {
                IsSuccess = true
            };

            var inputModel = new SpgatewayInputModel
            {
                MerchantID = _bankInfoModel.MerchantID,
                Version = version
            };

            // 將model 轉換為List<KeyValuePair<string, string>>, null值不轉
            List<KeyValuePair<string, string>> tradeData = LambdaUtil.ModelToKeyValuePairList<TradeInfo>(tradeInfo);
            // 將List<KeyValuePair<string, string>> 轉換為 key1=Value1&key2=Value2&key3=Value3...
            var tradeQueryPara = string.Join("&", tradeData.Select(x => $"{x.Key}={x.Value}"));
            // AES 加密
            inputModel.TradeInfo =
                CryptoUtil.EncryptAESHex(tradeQueryPara, _bankInfoModel.HashKey, _bankInfoModel.HashIV);
            // SHA256 加密
            inputModel.TradeSha =
                CryptoUtil.EncryptSHA256(
                    $"HashKey={_bankInfoModel.HashKey}&{inputModel.TradeInfo}&HashIV={_bankInfoModel.HashIV}");

            // 將model 轉換為List<KeyValuePair<string, string>>, null值不轉
            List<KeyValuePair<string, string>> postData =
                LambdaUtil.ModelToKeyValuePairList<SpgatewayInputModel>(inputModel);

            Response.Clear();

            StringBuilder s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<form name='form' action='{0}' method='post'>", _bankInfoModel.AuthUrl);
            foreach (KeyValuePair<string, string> item in postData)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", item.Key, item.Value);
            }

            s.Append("</form></body></html>");
            byte[] bytes = Encoding.ASCII.GetBytes(s.ToString());
            await HttpContext.Response.Body.WriteAsync(bytes);
        }

    }
}

