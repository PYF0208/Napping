using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models.Entity;
using Napping_PJ.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Napping_PJ.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class PromotionsController : Controller
	{
		private readonly db_a989f8_nappingContext _context;

		public PromotionsController(db_a989f8_nappingContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		// GET: Admin/Promotions
		public async Task<IEnumerable<PromotionViewModel>> GetPromotionService()
		{
			var PromotionService = _context.Promotions.Select(PromotionService => new PromotionViewModel
			{
				PromotionId = PromotionService.PromotionId,
				LevelId = PromotionService.LevelId,
				Name = PromotionService.Name,
				StartDate = PromotionService.StartDate,
				EndDate = PromotionService.EndDate,
				Discount = PromotionService.Discount
			});
			return PromotionService;
		}
		[HttpGet]
		// GET: Admin/Promotions
		public async Task<IEnumerable<LevelViewModel>> GetLevel()
		{
			var Level = await _context.Levels
				.Select(Level => new LevelViewModel
				{
					LevelId = Level.LevelId,
					Name = Level.Name

				})
				.ToListAsync();

			return Level;
		}
		[HttpPost]
		public async Task<IEnumerable<PromotionViewModel>> FilterPromotions(
			[FromBody] PromotionViewModel prViewModel)
		{
			return _context.Promotions.Where(pro=>pro.Name.Contains(prViewModel.Name)||pro.Discount==prViewModel.Discount||pro.Level.Name.Contains(prViewModel.LevelName))
				.Select(pro => new PromotionViewModel
			{
				PromotionId = pro.PromotionId,
				LevelId = pro.LevelId,
				Name = pro.Name,
				StartDate = pro.StartDate,
				EndDate = pro.EndDate,
				Discount = pro.Discount,
				LevelName=pro.Level.Name
			});
		}



		[HttpPut]
		public async Task<string> PutPromotions(int id, [FromBody] PromotionViewModel promotion)
		{
			if (id != promotion.PromotionId)
			{
				return "修改促銷記錄失敗!";
			}
			int result = DateTime.Compare(promotion.StartDate, promotion.EndDate);
			if (result>=0)
			{
				return "結束日期必須大於起始日期!";
			}
			Promotion pro = await _context.Promotions.FindAsync(id);
			pro.PromotionId = promotion.PromotionId;
			pro.LevelId = promotion.LevelId;
			pro.Name = promotion.Name;
			pro.StartDate = promotion.StartDate;
			pro.EndDate = promotion.EndDate;
			pro.Discount = promotion.Discount;
			_context.Entry(pro).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PromotionExists(id))
				{
					return "修改促銷記錄失敗!";
				}
				else
				{
					throw;
				}
			}

			return "修改促銷記錄成功!";
		}
		[HttpPost]
		public string SendMailByPromotionId([FromBody] SendemailViewModel sendmailviewmodel)
		{
			var x = _context.Promotions.Include(x => x.Level).ThenInclude(x => x.Customers).FirstOrDefault(s => s.PromotionId == sendmailviewmodel.PromotionId);
			if (x != null)
			{
				var y = x.Level.Customers.Select(x => x.Email).ToList();
				foreach (var email in y)
				{
					try
					{
						var mail = new MailMessage()
						{
							From = new MailAddress("tibameth101team3@gmail.com"),
							Subject = "Napping會員促銷",
							Body = "尊貴的會員您好:\r\n獻上此促銷",
							IsBodyHtml = true,
							BodyEncoding = Encoding.UTF8,
						};
						mail.To.Add(new MailAddress(email));
						using (var sm = new SmtpClient("smtp.gmail.com", 587)) //465 ssl
						{
							sm.EnableSsl = true;
							sm.Credentials = new NetworkCredential("tibameth101team3@gmail.com", "glyirsixoioagwmh");
							sm.Send(mail);
						}
						return "寄送成功";
					}
					catch (Exception)
					{
						
						throw;
						
					}
				}
				
			}
			return "寄送失敗";
		}

		// POST: Admin/Promotions/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		public async Task<string> PostPromotion([FromBody] PromotionViewModel promotion)
		{
			if (promotion == null)
			{
				return "欄位不可為空值!";
			}
		
			Promotion pro = new Promotion
			{

				PromotionId = promotion.PromotionId,
				LevelId = promotion.LevelId,
				Name = promotion.Name,
				StartDate = promotion.StartDate,
				EndDate = promotion.EndDate,
				Discount = promotion.Discount,


			};
			int result = DateTime.Compare(pro.StartDate, pro.EndDate);
			if (result >= 0)
			{
				return "結束日期必須大於起始日期!";
			}
			_context.Promotions.Add(pro);
			await _context.SaveChangesAsync();

			return $"促銷編號:{pro.PromotionId}";
		}



		// Delete: Admin/Promotions/Delete/5
		[HttpDelete]
		public async Task<string> DeletePromotions(int id)
		{
			var promotion = await _context.Promotions.FindAsync(id);
			if (promotion == null)
			{
				return "刪除促銷記錄成功!";
			}

			_context.Promotions.Remove(promotion);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				return "刪除促銷關聯記錄失敗!";
			}

			return "刪除促銷記錄成功!";
		}

		private bool PromotionExists(int id)
		{
			return (_context.Promotions?.Any(e => e.PromotionId == id)).GetValueOrDefault();
		}

		public IActionResult Create()
		{
			return View();
		}
		
		
	}
}
