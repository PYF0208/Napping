using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using Napping_PJ.Services;
using System.Security.Claims;

namespace Napping_PJ.Controllers
{
    public class MemberController : Controller
    {
        private readonly db_a989f8_nappingContext _context;
		private readonly IBackgroundJobClient background;
		private readonly IBirthday birthday;

		public MemberController(db_a989f8_nappingContext context,IBackgroundJobClient background,IBirthday birthday)
        {
            _context = context;
			this.background = background;
			this.birthday = birthday;
		}

        public async Task<IActionResult> Index()
        {
            Customer customer = await _context.Customers.Include(x=>x.Level).FirstOrDefaultAsync(c => c.Email == User.FindFirstValue(ClaimTypes.Email));
            MemberViewModel memberViewModel = new MemberViewModel()
            {
                Email = customer.Email,
                Name = customer.Name,
                Birthday = customer.Birthday,
                Gender = customer.Gender,
                Phone = customer.Phone,
                City = customer.City,
                Region = customer.Region,
                Country = customer.Country,
                LevelName = customer.Level.Name,
            };
            return View(memberViewModel);

        }
        [HttpPost]
        public async Task<IActionResult> EditMemberInfo([FromBody] MemberViewModel memberViewModel)
        {
            if (!(User.FindFirstValue(ClaimTypes.Email) == memberViewModel.Email))
            {
                return BadRequest("登入者與修改對象不同");
            }
            Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == User.FindFirstValue(ClaimTypes.Email));
            customer.Name = memberViewModel.Name;
            customer.Birthday = memberViewModel.Birthday;
            customer.Gender = memberViewModel.Gender;
            customer.Phone = memberViewModel.Phone;
            customer.City = memberViewModel.City;
            customer.Region = memberViewModel.Region;
            customer.Country = memberViewModel.Country;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("寫入失敗");
            }

            return View(memberViewModel);
        }
        [HttpPost]
		public IActionResult SendMail()
        {				
				var send=BackgroundJob.Enqueue(() => birthday.SendBirthDayMail());
            return Ok("寄信成功");
			  //這邊執行背景發送生日信
		}
	}
}
