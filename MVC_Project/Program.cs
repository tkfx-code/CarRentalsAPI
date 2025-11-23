using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddHttpClient<IClient, Client>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7064");
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

            //admin user and roles
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            //    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            //    //hardcode admin user for testing purposes
            //    string adminRole = "Admin";
            //    string adminEmail = "admin@example.com";
            //    string adminPassword = "Admin@123";

            //    //hardcode a "superuser"
            //    string superUserRole = "SuperUser";
            //    string superUserEmail = "superuser@example.com";
            //    string superUserPassword = "SuperUser@123";

            //    //check that user and role exists, create if not
            //    if (!await roleManager.RoleExistsAsync(adminRole))
            //    {
            //        await roleManager.CreateAsync(new IdentityRole(adminRole));
            //    }

            //    //check that superuser role exists, create if not
            //    if (!await roleManager.RoleExistsAsync(superUserRole))
            //    {
            //        await roleManager.CreateAsync(new IdentityRole(superUserRole));
            //    }

            //    var adminUser = await userManager.FindByEmailAsync(adminEmail);
            //    if (adminUser == null)
            //    {
            //        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            //        var result = await userManager.CreateAsync(adminUser, adminPassword);
            //        if (result.Succeeded)
            //        {
            //            await userManager.AddToRoleAsync(adminUser, adminRole);
            //        }
            //    }
            //    else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            //    {
            //        await userManager.AddToRoleAsync(adminUser, adminRole);
            //    }

            //    //Logic for SuperUser
            //    var superUser = await userManager.FindByEmailAsync(superUserEmail);
            //    if (superUser == null)
            //    {
            //        superUser = new IdentityUser { UserName = superUserEmail, Email = superUserEmail, EmailConfirmed = true };
            //        var result = await userManager.CreateAsync(superUser, superUserPassword);
            //        if (result.Succeeded)
            //        {
            //            await userManager.AddToRoleAsync(superUser, superUserRole);
            //        }
            //    } else if (!await userManager.IsInRoleAsync(superUser, superUserRole))
            //    {
            //        await userManager.AddToRoleAsync(superUser, superUserRole);
            //    }
            //}

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
            //app.MapRazorPages();

            app.Run();
        }
    }
}
