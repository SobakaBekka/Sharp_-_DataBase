using Microsoft.EntityFrameworkCore;
using OnlineSupermarket.Data;
using Oracle.ManagedDataAccess.Client; // Импорт Oracle драйвера

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database context
builder.Services.AddDbContext<OnlineSupermarketContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Проверка подключения к базе данных
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OnlineSupermarketContext>();
    try
    {
        context.Database.OpenConnection();  // Попытка открытия соединения
        Console.WriteLine("Подключение к базе данных установлено успешно!");
        context.Database.CloseConnection(); // Закрытие соединения
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
    }
}

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
