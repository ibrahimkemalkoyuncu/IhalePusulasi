using Mesfel.Data;
using Mesfel.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);

// Servisleri DI Container'a ekle
builder.Services.AddControllersWithViews();

// Entity Framework ve SQL Server ba�lant�s�
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// �hale hesaplama servisini ekle
builder.Services.AddScoped<IIhaleHesaplamaService, IhaleHesaplamaService>();
builder.Services.AddScoped<IKamuIhaleHesaplamaService, KamuIhaleHesaplamaService>();
builder.Services.AddScoped<IRiskAnalizService, RiskAnalizService>();
builder.Services.AddScoped<IIhaleKarsilastirmaService, IhaleKarsilastirmaService>();
builder.Services.AddScoped<IZamanSerisiAnalizService, ZamanSerisiAnalizService>();


builder.Services.AddScoped<IKamuIhaleHesaplamaService, KamuIhaleHesaplamaService>();
builder.Services.AddScoped<IRiskAnalizService, RiskAnalizService>();

// Session deste�i ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Memory cache ekle
builder.Services.AddMemoryCache();

// Logging konfig�rasyonu
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

var app = builder.Build();

// HTTP request pipeline konfig�rasyonu
if (!app.Environment.IsDevelopment())
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
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Veritaban� olu�tur ve migrasyonlar� uygula
        context.Database.EnsureCreated();

        // Seed data kontrol�
        if (!context.Ihaleler.Any())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritaban� olu�turulurken hata olu�tu.");
    }
}

app.Run();