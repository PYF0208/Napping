using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Web;
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
		public async Task<IActionResult> Index()
		{
			string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
			Customer loginedUsr = await _Context.Customers.FirstOrDefaultAsync(x => x.Email == userEmail);
			byte[] cartBytes = HttpContext.Session.Get($"{loginedUsr.CustomerId}_cartItem");
			if (cartBytes == null)
			{
				return BadRequest("購物車為空");
			}
			string json = System.Text.Encoding.UTF8.GetString(cartBytes);
			IEnumerable<RoomDetailViewModel> roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
			return View(roomDetailViewModels);
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
