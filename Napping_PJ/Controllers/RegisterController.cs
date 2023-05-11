using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Napping_PJ.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using Napping_PJ.Models;

namespace Napping_PJ.Controllers
{
    public class RegisterController : Controller
    {

        private readonly db_a989f8_nappingContext _context;
        public RegisterController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TryCreateCustomer([Bind("Email,Password,ConfirmPassword")] RegisterViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                //使用信箱尋找使用者是否存在，如不存在則創造一個，並傳回
                (List<UserRole> hasRoles, bool newUser) = CheckOAuthRecord(customerViewModel.Email, customerViewModel.Password);
                if (newUser)
                {
                    // 根據啟動檔案中的 o.DefaultScheme = "Application" 初始化聲明值
                    var claimsIdentity = new ClaimsIdentity("Application");
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, customerViewModel.Email));
                    foreach (var role in hasRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleId.ToString())); // 使用者的角色
                    }
                    await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Email", "該用戶已註冊。");
                }
            }
            return View("Index", customerViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> OauthLogout()
        {
            await HttpContext.SignOutAsync("Application");

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult OauthGooleLogin()
        {
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    // 設定 google responds 後調用的Action Method
                    RedirectUri = Url.Action("GoogleResponse", "Register")
                });
        }
        public async Task<IActionResult> GoogleResponse()
        {
            // 檢查驗證回應是否符合啟動檔案中指定的 o.DefaultSignInScheme = "External"
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded)
                return BadRequest(); // TODO: 處理此錯誤的方式。
                                     // 檢查是否通過 Google 或其他連結進行重新導向
            string authType = authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower();
            if (authType == "google")
            {
                // 檢查主要值是否存在
                if (authenticateResult.Principal != null)
                {
                    //自訂方法，檢查是否已使用GoogleId註冊過
                    List<UserRole> hasRoles = CheckOAuthRecord(authenticateResult);
                    // 根據啟動檔案中的 o.DefaultScheme = "Application" 初始化聲明值
                    var claimsIdentity = new ClaimsIdentity("Application");
                    if (authenticateResult.Principal != null)
                    {
                        // 現在添加聲明值並重新導向到成功登錄後要訪問的頁面
                        var details = authenticateResult.Principal.Claims.ToList();
                        claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));// 使用者的ID
                        claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email)); // 使用者的電子郵件地址
                        foreach (var role in hasRoles)
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleId.ToString())); // 使用者的角色
                        }
                        await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private List<UserRole> CheckOAuthRecord(AuthenticateResult authenticateResult)
        {
            // 根據 id 取得 Google 帳戶 id，以執行任何基於該 id 的操作
            string googleAccountId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // 根據 id 取得 Google 帳戶 Email，以執行任何基於該 Email 的操作
            string googleAccountEmail = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            // 根據 id 取得 Google 帳戶 name，以執行任何基於該 name 的操作
            string googleAccountName = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
            // 根據 id 取得 Google 帳戶 Type，以執行任何基於該 Type 的操作
            string OAuthAccountType = authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower();
            //查詢新會員的ID
            Customer getCustomer = _context.Customers.FirstOrDefault(c => c.Email == googleAccountEmail);
            if (getCustomer == null)
            {
                Customer newCustomer = new Customer()
                {
                    Email = googleAccountEmail,
                    Name = googleAccountEmail
                };
                //寫入Customers資料表
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                //查詢新會員的ID
                getCustomer = _context.Customers.FirstOrDefault(c => c.Email == googleAccountEmail);
                //依資料模型創造物件
                Oauth newOauth = new Oauth()
                {
                    OauthId = googleAccountId,
                    Type = OAuthAccountType,
                    CustomerId = getCustomer.CustomerId
                };
                //寫入資料庫
                _context.Oauths.Add(newOauth);
                _context.SaveChanges();
            }
            List<UserRole> getOauth = _context.UserRoles.Where(ur => ur.Customer == getCustomer).ToList();
            return getOauth;
        }
        internal (List<UserRole>, bool) CheckOAuthRecord(string Email, string passWord)
        {
            bool newUser = false;
            //查詢新會員的ID
            Customer? getCustomer = _context.Customers.FirstOrDefault(c => c.Email == Email);
            if (getCustomer == null)
            {
                newUser = true;
                Customer newCustomer = new Customer()
                {
                    Name = Email,
                    Email = Email,
                    Password = passWord
                };
                //寫入Customers資料表
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                //查詢新會員的ID
                getCustomer = _context.Customers.FirstOrDefault(c => c.Email == Email);

                UserRole newUserRole = new UserRole()
                {
                    CustomerId = getCustomer.CustomerId,
                    RoleId = 3
                };
                _context.UserRoles.Add(newUserRole);
                _context.SaveChanges();
            }
            List<UserRole> getOauth = _context.UserRoles.Where(ur => ur.Customer == getCustomer).ToList();
            return (getOauth, newUser);
        }
    }
}
