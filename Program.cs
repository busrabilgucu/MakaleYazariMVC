using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sinav_Busra.Data; // DbContext'in bulunduðu namespace
using Sinav_Busra.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý (Connection string appsettings.json içinde olmalý)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yapýlandýrmasý
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Razor Pages ve Controller servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Hata sayfalarý ve güvenlik
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik doðrulama
app.UseAuthorization();  // Yetkilendirme

// MVC yönlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity için Razor Pages (Register, Login, vb.)
app.MapRazorPages();

app.Run();
