using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVC_Project.Data; 
using MVC_Project.Services;

namespace MVC_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(typeof(ApiMappingProfile));
            builder.Services.AddHttpClient<IClient, Client>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7064");
            });

            builder.Services.AddScoped<IAuthClientService, AuthClientService>((IServiceProvider provider) =>
                {
                var client = provider.GetRequiredService<IClient>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                return new AuthClientService(client, httpContextAccessor);
            });

            builder.Services.AddScoped<IBookingClientService, BookingClientService>();
            builder.Services.AddScoped<ICarClientService, CarClientService>();
            builder.Services.AddScoped<ICustomerClientService, CustomerClientService>();

            //Standard service lengths by VS
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
