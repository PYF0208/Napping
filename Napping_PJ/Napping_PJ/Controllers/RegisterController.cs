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
using Napping_PJ.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Napping_PJ.Controllers
{
    public class RegisterController : Controller
    {

        private readonly db_a989f8_nappingContext _context;
        private readonly EncryptHelper _encrypt;
        public RegisterController(db_a989f8_nappingContext context, EncryptHelper encrypt)
        {
            _context = context;
            _encrypt = encrypt;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ResendValidationEmail()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ValidField([FromBody][Bind("Email,Password,ConfirmPassword")] RegisterViewModel registerViewModel)
        {
            UserValidViewModel userValidViewModel = new UserValidViewModel()
            {
                mainError = null,
                emailError = null,
                passWordError = null,
                confirmPasswordError = null,
            };
            if (!ModelState.IsValid)
            {
                IEnumerable<string> emailErrors = ModelState["Email"]?.Errors.Select(e => e.ErrorMessage);
                IEnumerable<string> passwordErrors = ModelState["Password"]?.Errors.Select(e => e.ErrorMessage);
                IEnumerable<string> confirmPasswordError = ModelState["ConfirmPassword"]?.Errors.Select(e => e.ErrorMessage);

                userValidViewModel.mainError = null;
                userValidViewModel.emailError = emailErrors == null ? null : string.Join(", ", emailErrors);
                userValidViewModel.passWordError = passwordErrors == null ? null : string.Join(", ", passwordErrors);
                userValidViewModel.confirmPasswordError = confirmPasswordError == null ? null : string.Join(", ", confirmPasswordError);

                return BadRequest(userValidViewModel);
            }
            return Ok(userValidViewModel);
        }
        public async Task<IActionResult> TryRegister([FromBody][Bind("Email,Password,ConfirmPassword")] RegisterViewModel registerViewModel)
        {
            UserValidViewModel error = new UserValidViewModel
            {
                mainError = null,
                emailError = null,
                passWordError = null,
                confirmPasswordError = null
            };
            //確認Email是否已存在
            bool emailExist = await _context.Customers.AnyAsync(c => c.Email == registerViewModel.Email);
            if (emailExist)
            {
                error.emailError = "信箱已存在";
                return BadRequest(error);
            }
            //驗證資料是否符合RegisterViewModel規範
            if (!ModelState.IsValid)
            {
                IEnumerable<string> emailErrors = ModelState["Email"]?.Errors.Select(e => e.ErrorMessage);
                IEnumerable<string> passwordErrors = ModelState["Password"]?.Errors.Select(e => e.ErrorMessage);
                IEnumerable<string> confirmPasswordError = ModelState["ConfirmPassword"]?.Errors.Select(e => e.ErrorMessage);

                error.mainError = null;
                error.emailError = emailErrors == null ? null : string.Join(", ", emailErrors);
                error.passWordError = passwordErrors == null ? null : string.Join(", ", passwordErrors);
                error.confirmPasswordError = confirmPasswordError == null ? null : string.Join(", ", confirmPasswordError);

                return BadRequest(error);
            }
            // 创建新用户
            var newUser = new Customer
            {
                Name = registerViewModel.Email,
                Email = registerViewModel.Email,
                //密碼加鹽加密
                Password = PasswordHasher.HashPassword(registerViewModel.Password, registerViewModel.Email)
            };
            try
            {
                await _context.Customers.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                error.mainError = ex.InnerException?.Message;
                // 處理錯誤訊息...
                return BadRequest(error);
            }
            // 添加成功，可以獲取新的 ID
            Customer getCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == newUser.Email);

            // 添加默認角色到用戶角色表
            UserRole newUserRole = new UserRole()
            {
                RoleId = 3,
                CustomerId = getCustomer.CustomerId
            };
            try
            {
                await _context.UserRoles.AddAsync(newUserRole);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // 處理錯誤訊息...
                error.mainError = ex.InnerException?.Message;
                return BadRequest(error);
            }
            EmailValidViewModel emailValidViewModel = new EmailValidViewModel()
            {
                Email = registerViewModel.Email,
            };
            //寄送驗證信
            SendValidEmail(emailValidViewModel);

            return RedirectToAction("Index", "Home", new { area = "" });
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
                string passwodHash = PasswordHasher.HashPassword(passWord, Email);
                Customer newCustomer = new Customer()
                {
                    Name = Email,
                    Email = Email,
                    Password = passwodHash
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
        [HttpPost]
        public IActionResult ValidEmail([FromBody] EmailValidViewModel emailValidViewModel)
        {
            IEnumerable<string> emailErrors = null;
            if (!ModelState.IsValid)
            {
                emailErrors = ModelState["Email"]?.Errors.Select(e => e.ErrorMessage);

                return BadRequest(emailErrors == null ? null : string.Join(", ", emailErrors));
            }
            return Ok(emailErrors == null ? null : string.Join(", ", emailErrors));
        }
        [HttpPost]
        public IActionResult SendValidEmail([FromBody] EmailValidViewModel emailValidViewModel)
        {
            //資料驗證
            IEnumerable<string> emailErrors = null;
            if (!ModelState.IsValid)
            {
                emailErrors = ModelState["Email"]?.Errors.Select(e => e.ErrorMessage);

                return BadRequest(emailErrors == null ? null : string.Join(", ", emailErrors));
            }
            Customer getCustomer = _context.Customers.FirstOrDefault(c => c.Email == emailValidViewModel.Email);
            if (getCustomer != null)
            {
                if (getCustomer.Locked == false)
                {
                    return BadRequest("此帳號已通過驗證");
                }
            }
            //加密資料
            var obj = new AesValidationDto(emailValidViewModel.Email, DateTime.Now.AddDays(3));
            var jString = JsonSerializer.Serialize(obj);
            var code = _encrypt.AesEncryptToBase64(jString);
            string encodedStr = HttpUtility.UrlEncode(code);

            //寄送驗證信
            var mail = new MailMessage()
            {
                From = new MailAddress("tibameth101team3@gmail.com"),
                Subject = "Napping會員驗證信件",
                Body = $@"<!DOCTYPE html><html><head><meta charset=""UTF-8""><title>會員驗證</title></head><body><h1>會員驗證</h1><p>尊敬的會員：</p><p>感謝您註冊成為我們的會員。為了完成驗證過程，請點擊以下連結：</p><p><a href='https://localhost:7265/Register/ValidEmail?code={encodedStr}'>驗證連結</a></p><p>如果無法點擊上述連結，請將連結複製到瀏覽器地址欄中並訪問。</p><p>感謝您的支持！</p></body></html>",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
            };
            mail.To.Add(new MailAddress(emailValidViewModel.Email));
            try
            {
                using (var sm = new SmtpClient("smtp.gmail.com", 587)) //465 ssl
                {
                    sm.EnableSsl = true;
                    sm.Credentials = new NetworkCredential("tibameth101team3@gmail.com", "glyirsixoioagwmh");
                    sm.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok("驗證信已寄出");
        }
        public async Task<IActionResult> ValidEmail(string code)
        {
            var str = _encrypt.AesDecryptToString(code);
            var obj = JsonSerializer.Deserialize<AesValidationDto>(str);
            if (DateTime.Now > obj.ExpiredDate)
            {
                return BadRequest("過期");
            }
            var getCustomer = _context.Customers.FirstOrDefault(x => x.Email == obj.Email);
            if (getCustomer != null)
            {
                getCustomer.Locked = false;
                _context.SaveChanges();
            }

            //找到使用者擁有的權限
            IQueryable<UserRole> hasRoles = _context.UserRoles.Where(ur => ur.CustomerId == getCustomer.CustomerId);
            // 根據啟動檔案中的 o.DefaultScheme = "Application" 初始化聲明值
            var claimsIdentity = new ClaimsIdentity("Application");
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, getCustomer.Email));
            foreach (var role in hasRoles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleId.ToString())); // 使用者的角色
            }
            await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
            
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public IActionResult SendResetPasswordEmail([FromBody] EmailValidViewModel emailValidViewModel)
        {

            return Ok();
        }
    }
}
