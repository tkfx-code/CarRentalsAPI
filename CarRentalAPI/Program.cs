using System.Text;
using CarRentalAPI.Constants;
using CarRentalAPI.Data;
using CarRentalAPI.Interfaces;
using CarRentalAPI.Repositories;
using CarRentalAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext> (options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<APIUser>()
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

//Repositories
builder.Services.AddScoped<ICarListingRepo, CarListingRepo>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IBookingRepo, BookingRepo>();

//Authentication service
builder.Services.AddScoped<IAuthService, JwtService>();

//Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy => policy
        .AllowAnyMethod()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

//JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //Values from appsettings.json
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
    };
});

var app = builder.Build();

//seeding an admin user to test functions
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<APIUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        //hard coded admin
        string adminRole = APIRoles.Admin;
        string adminEmail = "admin@email.com";
        string adminPassword = "Admin123!";

        //hard coded user
        string userRole = APIRoles.User;
        string userEmail = "user@email.com";
        string userPassword = "User123!";

        //Check if role exists
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        if (!await roleManager.RoleExistsAsync(userRole))
        {
            await roleManager.CreateAsync(new IdentityRole(userRole));
        }

        //Create admin and user if they do not exist
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var newAdminUser = new APIUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };
            var result = await userManager.CreateAsync(newAdminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, adminRole);
            }
        }
        var normalUser = await userManager.FindByEmailAsync(userEmail);
        if (normalUser == null)
        {
            var newUser = new APIUser
            {
                UserName = userEmail,
                Email = userEmail
            };
            var result = await userManager.CreateAsync(newUser, userPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, userRole);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }

}

    //Middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.MapOpenApi();
        app.UseSwaggerUI();
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

app.UseHttpsRedirection();

app.UseCors("AllowClient");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();