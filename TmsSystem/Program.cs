using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ========================================
//  Session Configuration 
// ========================================
builder.Services.AddDistributedMemoryCache(); // שמירה בזיכרון (לפרודקשן: Redis)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // ⏱️ 60 דקות חוסר פעילות
    options.Cookie.HttpOnly = true; // 🔒 אבטחה - מניעת גישת JavaScript
    options.Cookie.IsEssential = true; // ✅ חיוני לעבודת המערכת
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // 🔓 גם HTTP (לפיתוח)
    options.Cookie.Name = ".TmsSystem.Session"; // שם ייחודי
});

// ========================================
// 🔑 Cookie Authentication - 15 דקות
// ========================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        // ⏱️ תוקף Cookie - 60 דקות
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

        // 🔄 Sliding Expiration - מתחדש בכל בקשה
        options.SlidingExpiration = true;

        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // 🔓 גם HTTP
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = ".TmsSystem.Auth";
        options.Cookie.IsEssential = true; // ✅ חיוני!
    });


// ========================================
// 🗄️ Database - MySQL
// ========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    ));

// ========================================
// 👤 Identity Configuration
// ========================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // הגדרות סיסמה (אופציונלי)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // הגדרות נעילה (אופציונלי)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;


    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// הגדרת Cookie של Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // ⏱️ תוקף - 60 דקות
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.IsEssential = true;
});

// ========================================
// 📧 Email Services
// ========================================
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, SendGridEmailService>();
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
builder.Services.AddScoped<OfferEmailSender>();
builder.Services.AddScoped<TripOfferEmailSender>();
builder.Services.AddScoped<TripEmailSender>();

// ========================================
// 📄 PDF Services
// ========================================
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<ITripOfferPdfService, TripOfferPdfService>();

// ========================================
// 🔐 Authorization
// ========================================
builder.Services.AddAuthorization();

// ========================================
var app = builder.Build();
// ========================================

// ========================================
// 👥 יצירת תפקידים ברירת מחדל
// ========================================
app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
});

// ========================================
// ⚙️ Middleware Pipeline - חשוב הסדר!
// ========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ⚠️ חשוב! Session לפני Authentication
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// app.Urls.Clear();
 //app.Urls.Add("http://0.0.0.0:5000"); // HTTP - uncomment if needed

app.Run();