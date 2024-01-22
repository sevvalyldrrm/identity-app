using identity_app.Context;
using identity_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<IdentityContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DBConStr"));
});

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<IdentityContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireDigit = false;

	options.User.RequireUniqueEmail = true;

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //kitlenme s�resi
	options.Lockout.MaxFailedAccessAttempts = 5;  //kullan�c�ya verilen giri� hakk� (hesap kitleme)

});


builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/Login";
	options.AccessDeniedPath = "/Account/AccessDenied"; //yetkisiz yerlere girerken
	options.SlidingExpiration = true;
	options.ExpireTimeSpan = TimeSpan.FromDays(30); //uygulamaya giri� yapt�ktan 30 g�n sonra cokienin s�resi dolar (logout olmad��� s�rece)
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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

IdentitySeedData.IdentityTestUser(app);

app.Run();
