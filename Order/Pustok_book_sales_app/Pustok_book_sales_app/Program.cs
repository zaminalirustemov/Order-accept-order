using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pustok_book_sales_app.Areas.Manage.Services;
using Pustok_book_sales_app.Models;
using Pustok_book_sales_app.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<PustokDbContext>(opt =>
{
    opt.UseSqlServer("Server=LAPTOP-TTJ08ARE\\SQLEXPRESS;Database=Pustok_DataBase_ZR;Trusted_Connection=True");
});
builder.Services.AddIdentity<AppUser,IdentityRole>(opt=>
{
    opt.Password.RequiredUniqueChars = 0;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireDigit = true;

    opt.User.RequireUniqueEmail = false;
}).AddEntityFrameworkStores<PustokDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<MemberLayoutService>();
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


app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
