using OnlineSupermarket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Налаштування логування для детального перегляду SQL-запитів (опціонально)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Додаємо AppDbContext до служб
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging() // Включає логування конфіденційних даних для відлагодження
           .EnableDetailedErrors());     // Включає детальні повідомлення про помилки

// Додаємо інші служби
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Налаштування конвеєра запитів HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Перевірка підключення до бази даних при запуску (опціонально)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.CanConnect();
        Console.WriteLine("Підключення до бази даних успішне.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка підключення до бази даних: {ex.Message}");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
