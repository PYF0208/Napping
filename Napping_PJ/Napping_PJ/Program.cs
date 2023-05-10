using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models.Entity;

namespace Napping_PJ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("db_a989f8_napping_admin") ?? throw new InvalidOperationException("Connection string 'db_a989f8_napping_admin' not found.");
            builder.Services.AddDbContext<db_a989f8_nappingContext>(options =>
                options.UseSqlServer(connectionString));            

            builder.Services.AddControllersWithViews();

            // 加入身份驗證服務
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = "Application";
                o.DefaultSignInScheme = "External";
            })
            .AddCookie("Application", options =>
            {
                options.LoginPath = "/Register/Register";
                options.AccessDeniedPath = "/Path/To/Your/AccessDeniedPage";
            })
            .AddCookie("External")
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
            });

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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

<<<<<<< HEAD

=======
>>>>>>> f670704fae36fc0bfb64f40b217222e71eba10a4
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
<<<<<<< HEAD
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
=======
                  name: "Admin",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
>>>>>>> f670704fae36fc0bfb64f40b217222e71eba10a4
            });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}