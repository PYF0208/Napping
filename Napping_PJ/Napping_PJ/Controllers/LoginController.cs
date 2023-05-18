using Microsoft.AspNetCore.Mvc;
using Napping_PJ.Models.Entity;
using Napping_PJ.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

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
        public IActionResult ResendValidationEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidField([FromBody][Bind("Email,Password")] LoginViewModel loginViewModel)
        {
            UserValidViewModel error = new UserValidViewModel
            {
                mainError = null,
                emailError = null,
                passWordError = null,
                confirmPasswordError = null
            };
            if (!ModelState.IsValid)
            {
                IEnumerable<string> emailErrors = ModelState["Email"]?.Errors.Select(e => e.ErrorMessage);
                IEnumerable<string> passwordErrors = ModelState["Password"]?.Errors.Select(e => e.ErrorMessage);

                error.mainError = null;
                error.emailError = emailErrors == null ? null : string.Join(", ", emailErrors);
                error.passWordError = passwordErrors == null ? null : string.Join(", ", passwordErrors);

                return BadRequest(error);
            }
            return Ok(error);
        }
        [HttpPost]
        public async Task<IActionResult> TryLogin([FromBody][Bind("Email,Password")] LoginViewModel loginViewModel)
        {
            UserValidViewModel error = new UserValidViewModel
            {
                mainError = null,
                emailError = null,
                passWordError = null,
                confirmPasswordError = null
            };
            if (ModelState.IsValid)
            {
                Customer? getCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == loginViewModel.Email);
                bool isValid = PasswordHasher.VerifyPassword(loginViewModel.Password, getCustomer.Email, getCustomer.Password);
                if (isValid)
                {
                    IQueryable<UserRole> hasRoles = _context.UserRoles.Where(ur => ur.CustomerId == getCustomer.CustomerId);
                    // 根據啟動檔案中的 o.DefaultScheme = "Application" 初始化聲明值
                    var claimsIdentity = new ClaimsIdentity("Application");
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, loginViewModel.Email));
                    foreach (var role in hasRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleId.ToString())); // 使用者的角色
                    }
                    await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
                    //return RedirectToAction("Index", "Home", new { area = "" });
                    return Ok();

                }
                else
                {
                    //ModelState.AddModelError("Email", "輸入的帳號或密碼有誤。");
                    error.mainError = "帳號或密碼錯誤";
                    return BadRequest(error);
                }
            }
            IEnumerable<string> emailErrors = ModelState["Email"]?.Errors.Select(e => e.ErrorMessage);
            IEnumerable<string> passwordErrors = ModelState["Password"]?.Errors.Select(e => e.ErrorMessage);

            error.mainError = null;
            error.emailError = emailErrors == null ? null : string.Join(", ", emailErrors);
            error.passWordError = passwordErrors == null ? null : string.Join(", ", passwordErrors);

            return BadRequest(error);
        }
    }
}
