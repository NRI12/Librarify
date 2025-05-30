using libraryproject.Data;
using libraryproject.Middleware;
using libraryproject.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<QLTVContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QLTVConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseSession();
app.UseRouting();
app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


app.MapControllerRoute(
    name: "search",
    pattern: "tim-kiem",
    defaults: new { controller = "TaiLieu", action = "Search" });

app.MapControllerRoute(
    name: "bookDetails",
    pattern: "tai-lieu/{id}/{slug?}",
    defaults: new { controller = "TaiLieu", action = "Details" });

app.MapControllerRoute(
    name: "readOnline",
    pattern: "doc-truc-tuyen/{id}/{slug?}",
    defaults: new { controller = "TaiLieu", action = "ReadOnline" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");