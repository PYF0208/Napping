using Microsoft.AspNetCore.Mvc;
using Napping_PJ.Models.Entity;
using Napping_PJ.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Napping_PJ.Controllers
{
    public class LoginController : Controller
    {
        private readonly db_a989f8_nappingContext _context;
        public LoginController(db_a989f8_nappingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> TryLogin(CustomerViewModel customerViewModel)
        {
            Customer getCustomer = await _context.Customers.FirstAsync(c => c.Email == customerViewModel.Email && c.Password == customerViewModel.Password);
            if (getCustomer != null)
            {
                IQueryable<UserRole> hasRoles = _context.UserRoles.Where(ur => ur.CustomerId == getCustomer.CustomerId);
                    // 根據啟動檔案中的 o.DefaultScheme = "Application" 初始化聲明值
                    var claimsIdentity = new ClaimsIdentity("Application");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, customerViewModel.Email));
                foreach (var role in hasRoles)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleId.ToString())); // 使用者的角色
                }
                await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));

            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
