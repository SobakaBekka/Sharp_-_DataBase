using OnlineSupermarket.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DatabaseHelper (чтобы он создавался на каждый запрос)
builder.Services.AddScoped<DatabaseHelper>();

// Register IPasswordHasher for RegisUzivatel
builder.Services.AddScoped<IPasswordHasher<RegisUzivatel>, PasswordHasher<RegisUzivatel>>();

// Если есть какие-то настройки логирования или конфигурации, добавьте их здесь.
// Например, настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();