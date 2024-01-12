using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using EmployeeSkillManagement.Data;
using EmployeeSkillManagement.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework Core DbContext using SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("localDB")));

// Add Identity services
builder.Services.AddIdentity<Admin, IdentityRole>(options =>
{
    // Configure Identity options
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure cookie options for authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Use exception handling middleware for non-development environments
    app.UseExceptionHandler("/Home/Error");

    // Enable HTTP Strict Transport Security (HSTS) for improved security
    app.UseHsts();
}

// Enable HTTPS redirection for added security
app.UseHttpsRedirection();

// Serve static files (e.g., CSS, images) from the wwwroot folder
app.UseStaticFiles();

// Enable routing for MVC controllers
app.UseRouting();

// Enable authentication
app.UseAuthentication();

// Enable authorization
app.UseAuthorization();

// Map the default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// // Map areas for additional controller routes
// app.MapControllerRoute(
//     name: "areas",
//     pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();
