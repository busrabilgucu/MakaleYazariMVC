using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sinav_Busra.Data; // DbContext'in bulundu�u namespace
using Sinav_Busra.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s� (Connection string appsettings.json i�inde olmal�)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yap�land�rmas�
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Razor Pages ve Controller servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Hata sayfalar� ve g�venlik
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik do�rulama
app.UseAuthorization();  // Yetkilendirme

// MVC y�nlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity i�in Razor Pages (Register, Login, vb.)
app.MapRazorPages();

app.Run();
