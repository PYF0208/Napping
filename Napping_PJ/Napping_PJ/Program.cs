using Hangfire;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Controllers;
using Napping_PJ.Helpers;
using Napping_PJ.Models.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Hangfire.SqlServer;
using Napping_PJ.Services;

namespace Napping_PJ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("AzureJP") ?? throw new InvalidOperationException("Connection string 'AzureJP' not found.");
            builder.Services.AddDbContext<db_a989f8_nappingContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllersWithViews();
            //�w���H�e�ͤ�H
			builder.Services.AddHangfire(configuration => configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseSqlServerStorage(builder.Configuration.GetConnectionString("AzureJP")));
			builder.Services.AddHangfireServer();
			builder.Services.AddTransient<IBirthday, Birthday>();
			// �[�J�������ҪA��
			builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = "Application";
                o.DefaultSignInScheme = "External";
            })
            .AddCookie("Application", options =>
            {
                options.LoginPath = "/Login/Index";
                options.AccessDeniedPath = "/Path/To/Your/AccessDeniedPage";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            })
            .AddCookie("External")
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
            });
            //�K�[session�A��
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // �ɮ�30�����L��
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
			app.UseHangfireDashboard(); //�w���H�e�ͤ�H
			app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //�Ұ�session
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
            app.Run();
        }
    }
}