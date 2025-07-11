using Mesfel;
using Mesfel.Data;
using Mesfel.Models;
using Mesfel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);

// Servisleri DI Container'a ekle
builder.Services.AddControllersWithViews();

// Entity Framework ve SQL Server ba�lant�s�
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Servisler
builder.Services.AddScoped<IIhaleHesaplamaService, IhaleHesaplamaService>();
builder.Services.AddScoped<IKamuIhaleHesaplamaService, KamuIhaleHesaplamaService>();
builder.Services.AddScoped<IRiskAnalizService, RiskAnalizService>();
builder.Services.AddScoped<IIhaleKarsilastirmaService, IhaleKarsilastirmaService>();
builder.Services.AddScoped<IZamanSerisiAnalizService, ZamanSerisiAnalizService>();
builder.Services.AddScoped<IIhaleAnalizService, IhaleAnalizService>();
builder.Services.AddScoped<IIhaleService, IhaleService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Session deste�i
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Memory cache
builder.Services.AddMemoryCache();

// Logging konfig�rasyonu
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// HTTP request pipeline konfig�rasyonu
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Route konfig�rasyonu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Veritaban� olu�turma ve seed data
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<ApplicationDbContext>();
//        context.Database.Migrate(); // Migration'lar� uygula
//        SeedData.Initialize(services); // Seed verilerini ekle
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "Veritaban� olu�turulurken veya seed data eklenirken hata olu�tu.");
//    }
//}

// Program.cs i�inde
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed verileri eklenirken bir hata olu�tu.");
    }
}

app.Run();