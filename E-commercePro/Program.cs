using E_commercePro.Data;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.Cookies;

using E_commercePro.services;

using E_commercePro.Services;


var builder = WebApplication.CreateBuilder(args);


// Configure Google authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;


})
.AddCookie();



// Configure SmtpSettings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddScoped<IEmailSender, EmailSender>();


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


builder.Services.AddHostedService<OtpCleanupService>();

// Inside ConfigureServices method



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "getSalesChartData",
        pattern: "AdDashboard/GetSalesChartData/{filter}/{time}",
        defaults: new { controller = "AdDashboard", action = "GetSalesChartData" });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=UserHome}/{id?}");

app.Run();


