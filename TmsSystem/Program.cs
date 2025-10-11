using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.Services;
using DinkToPdf;
using DinkToPdf.Contracts;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// שירות PDF
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IPdfService, PdfService>();

// הגדרות SMTP
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

// שירותי דוא"ל
builder.Services.AddSingleton<IEmailService, GmailSmtpEmailService>();
builder.Services.AddScoped<IHtmlEmailService, HtmlEmailService>();
builder.Services.AddScoped<OfferEmailSender>();

// חיבור ל-MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

var app = builder.Build();

// יצירת תפקידים ברירת מחדל
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User" };

    foreach (string role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();