using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Napping_PJ.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Napping_PJ.Models;
using Napping_PJ.Helpers;
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
        //public IActionResult OauthGooleLogin()
        //{
        //    return new ChallengeResult(
        //        GoogleDefaults.AuthenticationScheme,
        //        new AuthenticationProperties
        //        {
        //            // 設定 google responds 後調用的Action Method
        //            RedirectUri = Url.Action("GoogleResponse", "Register")
        //        });
        //}
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    // 檢查驗證回應是否符合啟動檔案中指定的 o.DefaultSignInScheme = "External"
        //    var authenticateResult = await HttpContext.AuthenticateAsync("External");
        //    if (!authenticateResult.Succeeded)
        //        return BadRequest(); // TODO: 處理此錯誤的方式。
        //                             // 檢查是否通過 Google 或其他連結進行重新導向
        //    string authType = authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower();
        //    if (authType == "google")
        //    {
        //        // 檢查主要值是否存在
        //        if (authenticateResult.Principal != null)
        //        {
        //            //自訂方法，檢查是否已使用GoogleId註冊過
        //            List<UserRole> hasRoles = CheckOAuthRecord(authenticateResult);
        //            // 根據啟動檔案中的 o.DefaultScheme = "Application" 初始化聲明值
        //            var claimsIdentity = new ClaimsIdentity("Application");
        //            if (authenticateResult.Principal != null)
        //            {
        //                // 現在添加聲明值並重新導向到成功登錄後要訪問的頁面
        //                var details = authenticateResult.Principal.Claims.ToList();
        //                claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));// 使用者的ID
        //                claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email)); // 使用者的電子郵件地址
        //                foreach (var role in hasRoles)
        //                {
        //                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleId.ToString())); // 使用者的角色
        //                }
        //                await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
        //                return RedirectToAction("Index", "Home");
        //            }
        //        }
        //    }
        //    return RedirectToAction("Index", "Home");
        //}
        //private List<UserRole> CheckOAuthRecord(AuthenticateResult authenticateResult)
        //{
        //    // 根據 id 取得 Google 帳戶 id，以執行任何基於該 id 的操作
        //    string googleAccountId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    // 根據 id 取得 Google 帳戶 Email，以執行任何基於該 Email 的操作
        //    string googleAccountEmail = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        //    // 根據 id 取得 Google 帳戶 name，以執行任何基於該 name 的操作
        //    string googleAccountName = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
        //    // 根據 id 取得 Google 帳戶 Type，以執行任何基於該 Type 的操作
        //    string OAuthAccountType = authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower();
        //    //查詢新會員的ID
        //    Customer getCustomer = _context.Customers.FirstOrDefault(c => c.Email == googleAccountEmail);
        //    if (getCustomer == null)
        //    {
        //        Customer newCustomer = new Customer()
        //        {
        //            Email = googleAccountEmail,
        //            Name = googleAccountEmail
        //        };
        //        //寫入Customers資料表
        //        _context.Customers.Add(newCustomer);
        //        _context.SaveChanges();
        //        //查詢新會員的ID
        //        getCustomer = _context.Customers.FirstOrDefault(c => c.Email == googleAccountEmail);
        //        //依資料模型創造物件
        //        Oauth newOauth = new Oauth()
        //        {
        //            OauthId = googleAccountId,
        //            Type = OAuthAccountType,
        //            CustomerId = getCustomer.CustomerId
        //        };
        //        //寫入資料庫
        //        _context.Oauths.Add(newOauth);
        //        _context.SaveChanges();
        //    }
        //    List<UserRole> getOauth = _context.UserRoles.Where(ur => ur.Customer == getCustomer).ToList();
        //    return getOauth;
        //}
        //internal (List<UserRole>, bool) CheckOAuthRecord(string Email, string passWord)
        //{
        //    bool newUser = false;
        //    //查詢新會員的ID
        //    Customer? getCustomer = _context.Customers.FirstOrDefault(c => c.Email == Email);
        //    if (getCustomer == null)
        //    {
        //        newUser = true;
        //        string passwodHash = PasswordHasher.HashPassword(passWord, Email);
        //        Customer newCustomer = new Customer()
        //        {
        //            Name = Email,
        //            Email = Email,
        //            Password = passwodHash
        //        };
        //        //寫入Customers資料表
        //        _context.Customers.Add(newCustomer);
        //        _context.SaveChanges();
        //        //查詢新會員的ID
        //        getCustomer = _context.Customers.FirstOrDefault(c => c.Email == Email);

        //        UserRole newUserRole = new UserRole()
        //        {
        //            CustomerId = getCustomer.CustomerId,
        //            RoleId = 3
        //        };
        //        _context.UserRoles.Add(newUserRole);
        //        _context.SaveChanges();
        //    }
        //    List<UserRole> getOauth = _context.UserRoles.Where(ur => ur.Customer == getCustomer).ToList();
        //    return (getOauth, newUser);
        //}
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
                Body = $@"<!DOCTYPE html>
<html xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" lang=""en"">

<head>
	<title></title>
	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
	<meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->
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

		@media (max-width:660px) {{

			.desktop_hide table.icons-inner,
			.social_block.desktop_hide .social-table {{
				display: inline-block !important;
			}}

			.icons-inner {{
				text-align: center;
			}}

			.icons-inner td {{
				margin: 0 auto;
			}}

			.image_block img.big,
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
		}}
	</style>
</head>

<body style=""background-color: #f8f8f9; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;"">
	<table class=""nl-container"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f8f8f9;"">
		<tbody>
			<tr>
				<td>
					<table class=""row row-1"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #1aa19c;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; background-color: #1aa19c; width: 640px;"" width=""640"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""divider_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 4px solid #1AA19C;""><span>&#8202;</span></td>
																		</tr>
																	</table>
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
					<table class=""row row-2"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fff;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fff; color: #000000; width: 640px;"" width=""640"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""image_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:25px;padding-top:22px;width:100%;padding-right:0px;padding-left:0px;"">
																<div class=""alignment"" align=""center"" style=""line-height:10px""><img src=""https://d1oco4z2z1fhwp.cloudfront.net/templates/default/1361/Companify-Logo.png"" style=""display: block; height: auto; border: 0; width: 149px; max-width: 100%;"" width=""149"" alt=""I'm an image"" title=""I'm an image""></div>
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
					<table class=""row row-3"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; background-color: #f8f8f9; width: 640px;"" width=""640"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""divider_block block-1"" width=""100%"" border=""0"" cellpadding=""20"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 0px solid #BBBBBB;""><span>&#8202;</span></td>
																		</tr>
																	</table>
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
					<table class=""row row-4"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fff; color: #000000; width: 640px;"" width=""640"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""divider_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:12px;padding-top:60px;"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 0px solid #BBBBBB;""><span>&#8202;</span></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
													<table class=""image_block block-2"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-left:40px;padding-right:40px;width:100%;"">
																<div class=""alignment"" align=""center"" style=""line-height:10px""><img class=""big"" src=""https://d1oco4z2z1fhwp.cloudfront.net/templates/default/1361/Img4_2x.jpg"" style=""display: block; height: auto; border: 0; width: 352px; max-width: 100%;"" width=""352"" alt=""I'm an image"" title=""I'm an image""></div>
															</td>
														</tr>
													</table>
													<table class=""divider_block block-3"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-top:50px;"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 0px solid #BBBBBB;""><span>&#8202;</span></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-4"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:10px;padding-left:40px;padding-right:40px;padding-top:10px;"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14.399999999999999px; color: #555555; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 16px; text-align: center; mso-line-height-alt: 19.2px;""><span style=""font-size:30px;color:#2b303a;""><strong>Activate your account with the activation link</strong></span></p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-5"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:10px;padding-left:40px;padding-right:40px;padding-top:10px;"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 18px; color: #555555; line-height: 1.5;"">
																		<p style=""margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 22.5px;""><span style=""color:#808389;font-size:15px;"">Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmodati mat tempor incididunt ut labore et dolore magna aliqua.</span></p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""divider_block block-6"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-top:50px;"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 0px solid #BBBBBB;""><span>&#8202;</span></td>
																		</tr>
																	</table>
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
					<table class=""row row-5"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fff; color: #000000; width: 640px;"" width=""640"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""divider_block block-1"" width=""100%"" border=""0"" cellpadding=""20"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 0px solid #BBBBBB;""><span>&#8202;</span></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
													<table class=""button_block block-2"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"">
																<div class=""alignment"" align=""center""><!--[if mso]><v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""https://localhost:7265/Register/ValidEmail?code={encodedStr}"" style=""height:62px;width:158px;v-text-anchor:middle;"" arcsize=""49%"" stroke=""false"" fillcolor=""#1aa19c""><w:anchorlock/><v:textbox inset=""0px,0px,0px,0px""><center style=""color:#ffffff; font-family:Tahoma, sans-serif; font-size:16px""><![endif]--><a href=""https://localhost:7265/Register/ValidEmail?code={encodedStr}"" target=""_blank"" style=""text-decoration:none;display:inline-block;color:#ffffff;background-color:#1aa19c;border-radius:30px;width:auto;border-top:0px solid transparent;font-weight:400;border-right:0px solid transparent;border-bottom:0px solid transparent;border-left:0px solid transparent;padding-top:15px;padding-bottom:15px;font-family:Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;font-size:16px;text-align:center;mso-border-alt:none;word-break:keep-all;""><span style=""padding-left:30px;padding-right:30px;font-size:16px;display:inline-block;letter-spacing:normal;""><span dir=""ltr"" style=""word-break: break-word; line-height: 32px;""><strong>Activate Link</strong></span></span></a><!--[if mso]></center></v:textbox></v:roundrect><![endif]--></div>
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
					<table class=""row row-6"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; background-color: #2b303a; width: 640px;"" width=""640"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""divider_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 4px solid #1AA19C;""><span>&#8202;</span></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
													<table class=""image_block block-2"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""width:100%;padding-right:0px;padding-left:0px;"">
																<div class=""alignment"" align=""center"" style=""line-height:10px""><img class=""big"" src=""https://d1oco4z2z1fhwp.cloudfront.net/templates/default/1361/footer.png"" style=""display: block; height: auto; border: 0; width: 640px; max-width: 100%;"" width=""640"" alt=""I'm an image"" title=""I'm an image""></div>
															</td>
														</tr>
													</table>
													<table class=""image_block block-3"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-top:40px;width:100%;padding-right:0px;padding-left:0px;"">
																<div class=""alignment"" align=""center"" style=""line-height:10px""><img src=""https://d1oco4z2z1fhwp.cloudfront.net/templates/default/1361/Logo-white.png"" style=""display: block; height: auto; border: 0; width: 149px; max-width: 100%;"" width=""149"" alt=""Alternate text"" title=""Alternate text""></div>
															</td>
														</tr>
													</table>
													<table class=""social_block block-4"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:28px;text-align:center;"">
																<div class=""alignment"" align=""center"">
																	<table class=""social-table"" width=""208px"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block;"">
																		<tr>
																			<td style=""padding:0 10px 0 10px;""><a href=""https://www.facebook.com/"" target=""_blank""><img src=""https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/facebook@2x.png"" width=""32"" height=""32"" alt=""Facebook"" title=""Facebook"" style=""display: block; height: auto; border: 0;""></a></td>
																			<td style=""padding:0 10px 0 10px;""><a href=""https://twitter.com/"" target=""_blank""><img src=""https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/twitter@2x.png"" width=""32"" height=""32"" alt=""Twitter"" title=""Twitter"" style=""display: block; height: auto; border: 0;""></a></td>
																			<td style=""padding:0 10px 0 10px;""><a href=""https://instagram.com/"" target=""_blank""><img src=""https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/instagram@2x.png"" width=""32"" height=""32"" alt=""Instagram"" title=""Instagram"" style=""display: block; height: auto; border: 0;""></a></td>
																			<td style=""padding:0 10px 0 10px;""><a href=""https://www.linkedin.com/"" target=""_blank""><img src=""https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-outline-circle-white/linkedin@2x.png"" width=""32"" height=""32"" alt=""LinkedIn"" title=""LinkedIn"" style=""display: block; height: auto; border: 0;""></a></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-5"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:10px;padding-left:40px;padding-right:40px;padding-top:15px;"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 18px; color: #555555; line-height: 1.5;"">
																		<p style=""margin: 0; font-size: 14px; text-align: left; mso-line-height-alt: 18px;""><span style=""color:#95979c;font-size:12px;"">Etiam quis tempus ex. Sed vitae ipsum suscipit, ultricies odio vitae, suscipit massa. Sed tempus ipsum eget diam aliquam maximus. Cras accumsan urna vel rutrum lobortis. Maecenas tristique purus vel ex tempor consequat. Curabitur dui massa, congue sed sem at, rhoncus imperdiet sem. Fusce ac orci fermentum, malesuada dolor a, cursus augue. Quisque porttitor sapien arcu, quis iaculis nisi faucibus eget. Vestibulum eu velit rhoncus, aliquam ante eget, tristique diam dui massa, congue sed sem at, rhoncus usce ac orci fermentum,.</span></p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""divider_block block-6"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:10px;padding-left:40px;padding-right:40px;padding-top:25px;"">
																<div class=""alignment"" align=""center"">
																	<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																		<tr>
																			<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 1px solid #555961;""><span>&#8202;</span></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-7"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:30px;padding-left:40px;padding-right:40px;padding-top:20px;"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 12px; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; mso-line-height-alt: 14.399999999999999px; color: #555555; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; text-align: left; mso-line-height-alt: 16.8px;""><span style=""color:#95979c;font-size:12px;"">Companify Copyright © 2020</span></p>
																	</div>
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
					<table class=""row row-7"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 640px;"" width=""640"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""icons_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;"">
																<table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																	<tr>
																		<td class=""alignment"" style=""vertical-align: middle; text-align: center;""><!--[if vml]><table align=""left"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""display:inline-block;padding-left:0px;padding-right:0px;mso-table-lspace: 0pt;mso-table-rspace: 0pt;""><![endif]-->
																			<!--[if !vml]><!-->
																			<table class=""icons-inner"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;"" cellpadding=""0"" cellspacing=""0"" role=""presentation""><!--<![endif]-->
																				<tr>
																					<td style=""vertical-align: middle; text-align: center; padding-top: 5px; padding-bottom: 5px; padding-left: 5px; padding-right: 6px;""><a href=""https://www.designedwithbee.com/?utm_source=editor&utm_medium=bee_pro&utm_campaign=free_footer_link"" target=""_blank"" style=""text-decoration: none;""><img class=""icon"" alt=""Designed with BEE"" src=""https://d1oco4z2z1fhwp.cloudfront.net/assets/bee.png"" height=""32"" width=""34"" align=""center"" style=""display: block; height: auto; margin: 0 auto; border: 0;""></a></td>
																					<td style=""font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif; font-size: 15px; color: #9d9d9d; vertical-align: middle; letter-spacing: undefined; text-align: center;""><a href=""https://www.designedwithbee.com/?utm_source=editor&utm_medium=bee_pro&utm_campaign=free_footer_link"" target=""_blank"" style=""color: #9d9d9d; text-decoration: none;"">Designed with BEE</a></td>
																				</tr>
																			</table>
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
            return Ok("驗證信件已寄出");
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
            //資料驗證
            IEnumerable<string> emailErrors = null;
            if (!ModelState.IsValid)
            {
                emailErrors = ModelState["Email"]?.Errors.Select(e => e.ErrorMessage);

                return BadRequest(emailErrors == null ? null : string.Join(", ", emailErrors));
            }
            Customer getCustomer = _context.Customers.FirstOrDefault(c => c.Email == emailValidViewModel.Email);
            if (getCustomer == null)
            {
                return BadRequest("無此使用者");
            }

            //加密資料
            var obj = new AesValidationDto(getCustomer.Email, DateTime.Now.AddDays(3));
            var jString = JsonSerializer.Serialize(obj);
            var code = _encrypt.AesEncryptToBase64(jString);
            string encodedStr = HttpUtility.UrlEncode(code);

            //寄送驗證信
            var mail = new MailMessage()
            {
                From = new MailAddress("tibameth101team3@gmail.com"),
                Subject = "Napping會員重設密碼信件",
                Body = $@"<!DOCTYPE html>
<html xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" lang=""zh-CN"">

<head>
	<title></title>
	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
	<meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->
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

		@media (max-width:620px) {{

			.desktop_hide table.icons-inner,
			.social_block.desktop_hide .social-table {{
				display: inline-block !important;
			}}

			.icons-inner {{
				text-align: center;
			}}

			.icons-inner td {{
				margin: 0 auto;
			}}

			.image_block img.big,
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
		}}
	</style>
</head>

<body style=""background-color: #d9dffa; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;"">
	<table class=""nl-container"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #d9dffa;"">
		<tbody>
			<tr>
				<td>
					<table class=""row row-1"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #cfd6f4;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px;"" width=""600"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-top: 20px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""image_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""width:100%;padding-right:0px;padding-left:0px;"">
																<div class=""alignment"" align=""center"" style=""line-height:10px""><img class=""big"" src=""https://d1oco4z2z1fhwp.cloudfront.net/templates/default/3991/animated_header.gif"" style=""display: block; height: auto; border: 0; width: 600px; max-width: 100%;"" width=""600"" alt=""Card Header with Border and Shadow Animated"" title=""Card Header with Border and Shadow Animated""></div>
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
					<table class=""row row-2"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #d9dffa; background-image: url('https://d1oco4z2z1fhwp.cloudfront.net/templates/default/3991/body_background_2.png'); background-position: top center; background-repeat: repeat;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px;"" width=""600"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 15px; padding-left: 50px; padding-right: 50px; padding-top: 15px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""text_block block-1"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #506bec; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; mso-line-height-alt: 16.8px;""><strong><span style=""font-size:38px;"">Forgot your password?</span></strong></p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-2"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #40507a; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; mso-line-height-alt: 16.8px;""><span style=""font-size:16px;"">Hey, we received a request to reset your password.</span></p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-3"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #40507a; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; mso-line-height-alt: 16.8px;""><span style=""font-size:16px;"">Let’s get you a new one!</span></p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""button_block block-4"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""padding-bottom:20px;padding-left:10px;padding-right:10px;padding-top:20px;text-align:left;"">
																<div class=""alignment"" align=""left""><!--[if mso]><v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""http://www.example.com/"" style=""height:46px;width:212px;v-text-anchor:middle;"" arcsize=""35%"" stroke=""false"" fillcolor=""#506bec""><w:anchorlock/><v:textbox inset=""5px,0px,0px,0px""><center style=""color:#ffffff; font-family:Arial, sans-serif; font-size:15px""><![endif]--><a href=""https://localhost:7265/Register/ResetPassword?code={encodedStr}"" target=""_blank"" style=""text-decoration:none;display:inline-block;color:#ffffff;background-color:#506bec;border-radius:16px;width:auto;border-top:0px solid TRANSPARENT;font-weight:undefined;border-right:0px solid TRANSPARENT;border-bottom:0px solid TRANSPARENT;border-left:0px solid TRANSPARENT;padding-top:8px;padding-bottom:8px;font-family:Helvetica Neue, Helvetica, Arial, sans-serif;font-size:15px;text-align:center;mso-border-alt:none;word-break:keep-all;""><span style=""padding-left:25px;padding-right:20px;font-size:15px;display:inline-block;letter-spacing:normal;""><span style=""word-break:break-word;""><span style=""line-height: 30px;"" data-mce-style><strong>RESET MY PASSWORD</strong></span></span></span></a><!--[if mso]></center></v:textbox></v:roundrect><![endif]--></div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-5"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #40507a; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; mso-line-height-alt: 16.8px;""><span style=""font-size:14px;"">Having trouble? <a href=""http://www.example.com/"" target=""_blank"" title=""@socialaccount"" style=""text-decoration: none; color: #40507a;"" rel=""noopener""><strong>@socialaccount</strong></a></span></p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-6"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #40507a; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; mso-line-height-alt: 16.8px;"">Didn’t request a password reset? You can ignore this message.</p>
																	</div>
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
					<table class=""row row-3"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px;"" width=""600"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""image_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""width:100%;padding-right:0px;padding-left:0px;"">
																<div class=""alignment"" align=""center"" style=""line-height:10px""><img class=""big"" src=""https://d1oco4z2z1fhwp.cloudfront.net/templates/default/3991/bottom_img.png"" style=""display: block; height: auto; border: 0; width: 600px; max-width: 100%;"" width=""600"" alt=""Card Bottom with Border and Shadow Image"" title=""Card Bottom with Border and Shadow Image""></div>
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
					<table class=""row row-4"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px;"" width=""600"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 20px; padding-left: 10px; padding-right: 10px; padding-top: 10px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""image_block block-1"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"">
																<div class=""alignment"" align=""center"" style=""line-height:10px""><a href=""http://www.example.com/"" target=""_blank"" style=""outline:none"" tabindex=""-1""><img src=""https://d1oco4z2z1fhwp.cloudfront.net/templates/default/3991/logo.png"" style=""display: block; height: auto; border: 0; width: 145px; max-width: 100%;"" width=""145"" alt=""Your Logo"" title=""Your Logo""></a></div>
															</td>
														</tr>
													</table>
													<table class=""social_block block-2"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"">
																<div class=""alignment"" align=""center"">
																	<table class=""social-table"" width=""72px"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block;"">
																		<tr>
																			<td style=""padding:0 2px 0 2px;""><a href=""https://www.instagram.com/"" target=""_blank""><img src=""https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-only-logo-dark-gray/instagram@2x.png"" width=""32"" height=""32"" alt=""Instagram"" title=""instagram"" style=""display: block; height: auto; border: 0;""></a></td>
																			<td style=""padding:0 2px 0 2px;""><a href=""https://www.twitter.com/"" target=""_blank""><img src=""https://app-rsrc.getbee.io/public/resources/social-networks-icon-sets/t-only-logo-dark-gray/twitter@2x.png"" width=""32"" height=""32"" alt=""Twitter"" title=""twitter"" style=""display: block; height: auto; border: 0;""></a></td>
																		</tr>
																	</table>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-3"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #97a2da; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;"">+(123) 456–7890</p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-4"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #97a2da; line-height: 1.2;"">
																		<p style=""margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;"">This link will expire in the next 24 hours.<br>Please feel free to contact us at email@yourbrand.com. </p>
																	</div>
																</div>
															</td>
														</tr>
													</table>
													<table class=""text_block block-5"" width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"">
														<tr>
															<td class=""pad"">
																<div style=""font-family: sans-serif"">
																	<div class style=""font-size: 14px; font-family: Helvetica Neue, Helvetica, Arial, sans-serif; mso-line-height-alt: 16.8px; color: #97a2da; line-height: 1.2;"">
																		<p style=""margin: 0; text-align: center; font-size: 12px; mso-line-height-alt: 14.399999999999999px;""><span style=""font-size:12px;"">Copyright© 2021 Your Brand.</span></p>
																		<p id=""m_8010100107078456808text01"" style=""margin: 0; text-align: center; font-size: 12px; mso-line-height-alt: 14.399999999999999px;""><span style=""font-size:12px;""><a href=""http://www.example.com/"" target=""_blank"" title=""Unsubscribe&nbsp;"" style=""text-decoration: underline; color: #97a2da;"" rel=""noopener"">Unsubscribe</a> |&nbsp;<a href=""http://www.example.com/"" target=""_blank"" title=""Manage your preferences"" style=""text-decoration: underline; color: #97a2da;"" rel=""noopener"">Manage your preferences</a>&nbsp;|&nbsp;<a href=""http://www.example.com/"" target=""_blank"" title=""Privacy Policy"" style=""text-decoration: underline; color: #97a2da;"" rel=""noopener"">Privacy Policy</a></span></p>
																	</div>
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
					<table class=""row row-5"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
						<tbody>
							<tr>
								<td>
									<table class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px;"" width=""600"">
										<tbody>
											<tr>
												<td class=""column column-1"" width=""100%"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"">
													<table class=""icons_block block-1"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
														<tr>
															<td class=""pad"" style=""vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;"">
																<table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"">
																	<tr>
																		<td class=""alignment"" style=""vertical-align: middle; text-align: center;""><!--[if vml]><table align=""left"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""display:inline-block;padding-left:0px;padding-right:0px;mso-table-lspace: 0pt;mso-table-rspace: 0pt;""><![endif]-->
																			<!--[if !vml]><!-->
																			<table class=""icons-inner"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;"" cellpadding=""0"" cellspacing=""0"" role=""presentation""><!--<![endif]-->
																				<tr>
																					<td style=""vertical-align: middle; text-align: center; padding-top: 5px; padding-bottom: 5px; padding-left: 5px; padding-right: 6px;""><a href=""https://www.designedwithbee.com/"" target=""_blank"" style=""text-decoration: none;""><img class=""icon"" alt=""Designed with BEE"" src=""https://d15k2d11r6t6rl.cloudfront.net/public/users/Integrators/BeeProAgency/53601_510656/Signature/bee.png"" height=""32"" width=""34"" align=""center"" style=""display: block; height: auto; margin: 0 auto; border: 0;""></a></td>
																					<td style=""font-family: Helvetica Neue, Helvetica, Arial, sans-serif; font-size: 15px; color: #9d9d9d; vertical-align: middle; letter-spacing: undefined; text-align: center;""><a href=""https://www.designedwithbee.com/"" target=""_blank"" style=""color: #9d9d9d; text-decoration: none;"">Designed with BEE</a></td>
																				</tr>
																			</table>
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
            return Ok("重設密碼信件已寄出");
        }
        public IActionResult ResetPassword(string code)
        {
            var str = _encrypt.AesDecryptToString(code);
            var obj = JsonSerializer.Deserialize<AesValidationDto>(str);
            if (DateTime.Now > obj.ExpiredDate)
            {
                return BadRequest("過期");
            }
            Customer getCustomer = _context.Customers.FirstOrDefault(x => x.Email == obj.Email);
            if (getCustomer == null)
            {
                return BadRequest("無此使用者");
            }
            RegisterViewModel customerViewModel = new RegisterViewModel()
            {
                Email = getCustomer.Email,
                Password = null,
                ConfirmPassword = null,
            };
            return View(customerViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> TryResetPassword([FromBody] RegisterViewModel registerViewModel)
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
            Customer getCustomer = _context.Customers.FirstOrDefault(c => c.Email == registerViewModel.Email);
            if (getCustomer == null)
            {
                userValidViewModel.mainError = "無此使用者";
                return BadRequest(userValidViewModel);
            }
            getCustomer.Password = PasswordHasher.HashPassword(registerViewModel.Password, registerViewModel.Email);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // 處理錯誤訊息...
                userValidViewModel.mainError = ex.InnerException?.Message;
                return BadRequest(userValidViewModel);
            }
            return Ok(userValidViewModel);
        }

    }
}
