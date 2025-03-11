using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020880.Shop.AppCodes;

namespace SV21T1020880.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews()
                .AddMvcOptions(option =>
                {
                    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "AuthenticationCookie";
                                option.LoginPath = "/Account/Login";
                                option.ExpireTimeSpan = TimeSpan.FromDays(360);
                            });
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                await next.Invoke();
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Product}/{action=Index}/{id?}");

            ApplicationContext.Configure
                (
                    context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                    enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
                );
            string connectionString = builder.Configuration.GetConnectionString("MaiDucDat_WebDB");
            SV21T1020880.BusinessLayers.Configuration.Initialize(connectionString);

            app.Run();
        }
    }
}
