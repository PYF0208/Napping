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
			return _context.Promotions.Where(pro => pro.Name.Contains(prViewModel.Name) || pro.Discount == prViewModel.Discount || pro.Level.Name.Contains(prViewModel.LevelName))
				.Select(pro => new PromotionViewModel
				{
					PromotionId = pro.PromotionId,
					LevelId = pro.LevelId,
					Name = pro.Name,
					StartDate = pro.StartDate,
					EndDate = pro.EndDate,
					Discount = pro.Discount,
					LevelName = pro.Level.Name
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
			if (result >= 0)
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
			var x = _context.Promotions
				.Include(x => x.Level)
				.ThenInclude(x => x.Customers)
				.FirstOrDefault(s => s.PromotionId == sendmailviewmodel.PromotionId);


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

							Body = $@"<!DOCTYPE html>

<html lang=""en"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:v=""urn:schemas-microsoft-com:vml"">
<head>
<title></title>
<meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type""/>
<meta content=""width=device-width, initial-scale=1.0"" name=""viewport""/><!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->
<style>
		* {{
			box-sizing: border-box;
		}}

		body {{
			margin: 0;
			padding: 0;
		}}

		a[x-apple-data-detectors] {{
			color: inherit !important;
			text-decoration: inherit !important;
		}}

		#MessageViewBody a {{
			color: inherit;
			text-decoration: none;
		}}

		p {{
			line-height: inherit
		}}

		.desktop_hide,
		.desktop_hide table {{
			mso-hide: all;
			display: none;
			max-height: 0px;
			overflow: hidden;
		}}

		.image_block img+div {{
			display: none;
		}}

		@media (max-width:720px) {{
			.desktop_hide table.icons-inner {{
				display: inline-block !important;
			}}

			.icons-inner {{
				text-align: center;
			}}

			.icons-inner td {{
				margin: 0 auto;
			}}

			.row-content {{
				width: 100% !important;
			}}

			.mobile_hide {{
				display: none;
			}}

			.stack .column {{
				width: 100%;
				display: block;
			}}

			.mobile_hide {{
				min-height: 0;
				max-height: 0;
				max-width: 0;
				overflow: hidden;
				font-size: 0px;
			}}

			.desktop_hide,
			.desktop_hide table {{
				display: table !important;
				max-height: none !important;
			}}

			.row-1 .column-2 .block-1.heading_block h1 {{
				font-size: 35px !important;
			}}
		}}
	</style>
</head>
<body style=""background-color: #f1e9da; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""nl-container"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f1e9da;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-size: auto;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-size: auto; background-color: #334259; border-radius: 0; color: #000000; border-left: 16px solid #E2D7C1; border-right: 16px solid #E2D7C1; width: 700px;"" width=""700"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: middle; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""50%"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""image_block block-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-left:20px;padding-right:20px;width:100%;"">
<div align=""center"" class=""alignment"" style=""line-height:10px""><img alt="" Image"" src=""https://cdn.pixabay.com/photo/2015/03/22/17/55/sale-685007_1280.jpg"" style=""display: block; height: auto; border: 0; width: 294px; max-width: 100%;"" title=""Father's Day Image"" width=""294""/></div>
</td>
</tr>
</table>
</td>
<td class=""column column-2"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: middle; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""50%"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""heading_block block-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:20px;padding-left:20px;padding-right:30px;padding-top:20px;text-align:center;width:100%;"">
<h1 style=""margin: 0; color: #d79c60; direction: ltr; font-family: 'Montserrat', 'Trebuchet MS', 'Lucida Grande', 'Lucida Sans Unicode', 'Lucida Sans', Tahoma, sans-serif; font-size: 40px; font-weight: 700; letter-spacing: normal; line-height: 120%; text-align: left; margin-top: 0; margin-bottom: 0;""><span class=""tinyMce-placeholder"">Happy Napping Day!</span></h1>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""paragraph_block block-2"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:10px;padding-left:20px;padding-right:30px;padding-top:10px;"">
<div style=""color:#e2d7c1;direction:ltr;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;font-size:18px;font-weight:400;letter-spacing:0px;line-height:150%;text-align:left;mso-line-height-alt:27px;"">
<p style=""margin: 0;"">尊貴的{sendmailviewmodel.LevelName}會員您好:<br>獻上{sendmailviewmodel.PromotionName}促銷<br>促銷折扣為:{sendmailviewmodel.Discount}<br>優惠起始時間為:{sendmailviewmodel.StartDate}<br>優惠結束時間為:{sendmailviewmodel.EndDate}
</p>
</div>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-2"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #334259; border-radius: 0; color: #000000; border-left: 16px solid #E2D7C1; border-right: 16px solid #E2D7C1; border-top: 0 solid #E2D7C1; width: 700px;"" width=""700"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<div class=""spacer_block block-1"" style=""height:20px;line-height:20px;font-size:1px;""> </div>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-3"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 700px;"" width=""700"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""icons_block block-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;"">
<table cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""alignment"" style=""vertical-align: middle; text-align: center;""><!--[if vml]><table align=""left"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""display:inline-block;padding-left:0px;padding-right:0px;mso-table-lspace: 0pt;mso-table-rspace: 0pt;""><![endif]-->
<!--[if !vml]><!-->

</td>
</tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table><!-- End -->
</body>
</html>",
							//$"尊貴的{sendmailviewmodel.LevelName}會員您好:<br>獻上{sendmailviewmodel.PromotionName}促銷<br>促銷折扣為:{sendmailviewmodel.Discount}<br>優惠起始時間為:{sendmailviewmodel.StartDate}<br>優惠結束時間為:{sendmailviewmodel.EndDate}",
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
						
					}
					catch (Exception)
					{

						throw;

					}
				}
				return "寄送成功";
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

	}
}
