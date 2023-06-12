using Hangfire;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Controllers;
using Napping_PJ.Helpers;
using Napping_PJ.Models.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hangfire.SqlServer;
using Napping_PJ.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Mail;

namespace Napping_PJ
{
    public class Program
    {
		//private readonly IBackgroundJobClient backgroundJobs;
		
		public static void Main(string[] args)
		{

			
			var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("AzureJP") ?? throw new InvalidOperationException("Connection string 'AzureJP' not found.");
            builder.Services.AddDbContext<db_a989f8_nappingContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllersWithViews();

            
            //定期寄送生日信
			builder.Services.AddHangfire(configuration => configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseSqlServerStorage(builder.Configuration.GetConnectionString("AzureJP")));
			builder.Services.AddHangfireServer();
            
			builder.Services.AddScoped<IBirthday, Birthday>();
            builder.Services.AddTransient<IChangePaymentStatusService, ChangePaymentStatusService>(); 
			// 加入身份驗證服務
			builder.Services.AddAuthentication(o =>
            {
                // 設置預設的身份驗證方案為 "Application"
                o.DefaultScheme = "Application";
                // 設置預設的外部登入方案為 "External"，供第三方登入使用
                o.DefaultSignInScheme = "External";
            })
            .AddCookie("Application", options =>
            {
                // 設置應用程式身份驗證的 Cookie 相關選項
                options.LoginPath = "/Login/Index"; // 登入頁面的路徑，如需要權限將轉向此頁面
                options.AccessDeniedPath = "/Path/To/Your/AccessDeniedPage"; // 拒絕存取的錯誤頁面的路徑
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie 的過期時間間隔為 30 分鐘
            })
            .AddCookie("External")// 添加外部身份驗證的 Cookie
            .AddGoogle(options =>
            {
                // 配置 Google 身份驗證提供者
                IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"]; // 設置 Google ClientId
                options.ClientSecret = googleAuthNSection["ClientSecret"]; // 設置 Google ClientSecret
            });
            //添加session服務
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // 時效30分鐘過期
            });

            builder.Services.AddTransient<EncryptHelper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
			app.UseHangfireDashboard(); //定期寄送生日信
			app.UseRouting();

            app.UseAuthentication();//啟用身份驗證
            app.UseAuthorization();//啟用身分授權
            //啟動session
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                 name: "areas",
                 pattern: "{area:exists}/{controller=Home}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

			});
			using (var Scope = app.Services.CreateScope())
			{
				var Services = Scope.ServiceProvider;
				var EmailSender = Services.GetRequiredService<IBirthday>();
				
				RecurringJob.AddOrUpdate(() => EmailSender.SendBirthDayMail(), Cron.Daily);
			}  //這邊執行背景發送生日信
			app.Run();
        }

	}
}