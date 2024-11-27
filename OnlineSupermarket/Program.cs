using OnlineSupermarket.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<DatabaseHelper>();
builder.Services.AddTransient<Adresa>();
builder.Services.AddTransient<Karta>();
builder.Services.AddTransient<Kategorie>();
builder.Services.AddTransient<Kupon>();
builder.Services.AddTransient<LogDatabaz>();
builder.Services.AddTransient<Platba>();
builder.Services.AddTransient<Pokladna>();
builder.Services.AddTransient<Pozice>();
builder.Services.AddTransient<ProdaneZbozi>();
builder.Services.AddTransient<Prodej>();
builder.Services.AddTransient<Prodejna>();
builder.Services.AddTransient<Pult>();
builder.Services.AddTransient<RegisUzivatel>();
builder.Services.AddTransient<Role>();
builder.Services.AddTransient<Sklad>();
builder.Services.AddTransient<Soubor>();
builder.Services.AddTransient<Zamestnanec>();
builder.Services.AddTransient<Zbozi>();
builder.Services.AddTransient<ZboziNaPulte>();
builder.Services.AddTransient<ZboziNaSklade>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
