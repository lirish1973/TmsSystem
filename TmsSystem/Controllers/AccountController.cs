using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Encodings.Web;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.Services;
using TmsSystem.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace TmsSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext,
            IEmailSender emailSender,
            IEmailService emailService,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = dbContext;
            _emailSender = emailSender;
            _emailService = emailService;
            _config = config;
        }

        // ===================== REGISTER =====================
        [HttpGet]
        // [Authorize(Roles = "Admin")] // רק מנהלים יכולים ליצור משתמשים חדשים
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.Phone,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                CompanyName = model.CompanyName,
                BirthDate = model.BirthDate,
                RegistrationDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // הוספת תפקיד למשתמש
                var roleToAssign = model.Role ?? "User";

                // בדיקה שרק מנהל יכול ליצור מנהל
                if (roleToAssign == "Admin" && !User.IsInRole("Admin"))
                {
                    roleToAssign = "User";
                }

                await _userManager.AddToRoleAsync(user, roleToAssign);

                // אם זה מנהל שיוצר משתמש, לא מחברים אותו אוטומטית
                if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                {
                    TempData["SuccessMessage"] = "המשתמש נוצר בהצלחה!";
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    // אם זה הרשמה רגילה, מחברים את המשתמש עם Session
                    // התחברות פשוטה בעזרת SignInManager (Identity)
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // שמירת נתונים ב־Session במידת הצורך
                    HttpContext.Session.SetString("Username", user.UserName ?? user.Email ?? "User");
                    HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString("O"));

                    return RedirectToAction("Index", "Home");
                }
            }

            // לוג מפורט של שגיאות
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error Code: {error.Code} | Description: {error.Description}");
            }
            ViewData["ErrorMessage"] = string.Join("<br>", result.Errors.Select(e => $"{e.Code}: {e.Description}"));

            return View(model);
        }

        // ===================== VIEW PROFILE (Read-Only) =====================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("לא נמצא משתמש מחובר.");
            }

            var model = new UserProfileViewModel
            {
                Id = user.Id ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                Address = user.Address ?? string.Empty,
                CompanyName = user.CompanyName ?? string.Empty,
                BirthDate = user.BirthDate,
                RegistrationDate = user.RegistrationDate
            };

            return View("ViewProfile", model);
        }

        // ===================== EDIT PROFILE =====================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("לא נמצא משתמש מחובר.");
            }

            var model = new UserProfileViewModel
            {
                Id = user.Id ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                Address = user.Address ?? string.Empty,
                CompanyName = user.CompanyName ?? string.Empty,
                BirthDate = user.BirthDate,
                RegistrationDate = user.RegistrationDate
            };

            return View("EditProfile", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // עדכון פרטי המשתמש
            user.UserName = model.Username;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.Phone;
            user.Address = model.Address;
            user.CompanyName = model.CompanyName;
            user.BirthDate = model.BirthDate;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // עדכון סיסמה אם הוזנה
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (passwordResult.Succeeded)
                    {
                        TempData["SuccessMessage"] = "הפרופיל והסיסמה עודכנו בהצלחה!";
                    }
                    else
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    TempData["SuccessMessage"] = "הפרופיל עודכן בהצלחה!";
                }

                return RedirectToAction("ViewProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // ===================== LOGIN - מעודכן עם Session =====================
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (Request.Query.ContainsKey("timeout"))
            {
                TempData["TimeoutMessage"] = "נותקת מהמערכת בעקבות חוסר פעילות למשך 15 דקות.";
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(TmsSystem.ViewModels.LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // חיפוש לפי אימייל; במקרה שלא נמצא - ננסה גם לפי שם משתמש (תאימות)
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.Email);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "שם משתמש/אימייל או סיסמה לא נכונים");
                return View(model);
            }

            // חשוב: משתמשים ב-UserName כאשר קוראים ל-PasswordSignInAsync
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, "שם משתמש/אימייל או סיסמה לא נכונים");
            return View(model);
        }



        // ===================== KEEP ALIVE - מאריך Session =====================
        [Authorize]
        [HttpPost]
        public IActionResult KeepAlive()
        {
            // עדכון זמן פעילות אחרון
            HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString("O"));
            return Ok(new
            {
                success = true,
                message = "Session extended",
                expiresIn = "15 minutes"
            });
        }

        // ===================== FORGOT PASSWORD =====================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // בדיקה פשוטה במסד הנתונים - טבלת aspnetusers
            var user = await _userManager.FindByEmailAsync(model.Email);

            // אם המשתמש לא קיים - עדיין נציג הודעת הצלחה (אבטחה)
            if (user == null)
            {
                ViewData["SuccessMessage"] = "אם כתובת האימייל קיימת במערכת, נשלח אליך קישור לאיפוס סיסמה.";
                return View();
            }

            try
            {
                // יצירת טוקן לאיפוס סיסמה
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                // יצירת קישור לאיפוס סיסמה
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { code = code, email = model.Email },
                    protocol: Request.Scheme);

                // יצירת תוכן HTML לאימייל - נשתמש בפונקציה הקיימת שלך
                var htmlBody = GenerateResetPasswordEmailHtml(user.UserName ?? user.Email, callbackUrl);
                var plainTextBody = $"שלום {user.UserName ?? user.Email},\n\n" +
                                   $"קיבלנו בקשה לאיפוס הסיסמה שלך.\n\n" +
                                   $"לחץ על הקישור הבא לאיפוס הסיסמה:\n{callbackUrl}\n\n" +
                                   $"הקישור תקף ל-24 שעות.\n\n" +
                                   $"אם לא ביקשת לאפס את הסיסמה, התעלם מהודעה זו.\n\n" +
                                   $"בברכה,\nצוות TMS System";

                // שליחת המייל באמצעות SendGrid - אותו שירות שעובד להצעות מחיר
                await _emailService.SendHtmlAsync(
                    toEmail: model.Email,
                    subject: "איפוס סיסמה - מערכת TMS",
                    htmlBody: htmlBody,
                    plainTextBody: plainTextBody
                );

                ViewData["SuccessMessage"] = "קישור לאיפוס סיסמה נשלח בהצלחה לכתובת המייל שלך!";
                //_logger.LogInformation($"Password reset email sent successfully to {model.Email}");
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, $"Error sending password reset email to {model.Email}");
                ViewData["ErrorMessage"] = "אירעה שגיאה בשליחת האימייל. אנא נסה שוב מאוחר יותר.";
            }

            return View();
        }

        // ===================== LOGOUT - מעודכן עם ניקוי Session =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // נקה Session
            HttpContext.Session.Clear();

            // התנתק
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "התנתקת בהצלחה מהמערכת.";

            return RedirectToAction("Index", "Home");
        }

        // ===================== PASSWORD RESET (Admin) =====================
        // ===================== PASSWORD RESET (Admin) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminResetPassword(string id)  // ✅ שינוי שם!
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא.";
                return RedirectToAction("Index", "Users");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "@#$TempPass123");

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"הסיסמה אופסה בהצלחה ל- '@#$TempPass123'.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("<br>", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Index", "Users");
        }


        // ===================== RESET PASSWORD (User - GET) =====================
        [HttpGet]
        public IActionResult ResetPassword(string code = null, string email = null)
        {
            if (code == null || email == null)
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = email
            };

            return View(model);
        }

        // ===================== RESET PASSWORD (User - POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // בדיקה שהמשתמש קיים במסד הנתונים
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewData["SuccessMessage"] = "הסיסמה אופסה בהצלחה! תוכל להתחבר כעת עם הסיסמה החדשה.";
                return View();
            }

            try
            {
                // פענוח הטוקן
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));

                // איפוס הסיסמה בטבלת aspnetusers
                var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

                if (result.Succeeded)
                {
                    ViewData["SuccessMessage"] = "הסיסמה אופסה בהצלחה! תוכל להתחבר כעת עם הסיסמה החדשה.";
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "אירעה שגיאה באיפוס הסיסמה. אנא נסה שוב.");
            }

            return View(model);
        }



        // ===================== עזר פרטי =====================
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        // ===================== יצירת HTML לאימייל =====================
        private string GenerateResetPasswordEmailHtml(string userName, string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            background-color: #0d6efd;
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .content {{
            padding: 40px 30px;
            text-align: right;
        }}
        .button {{
            display: inline-block;
            padding: 12px 30px;
            margin: 20px 0;
            background-color: #0d6efd;
            color: white !important;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            font-size: 12px;
            color: #6c757d;
        }}
        .warning {{
            background-color: #fff3cd;
            border-right: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>איפוס סיסמה</h1>
        </div>
        <div class='content'>
            <h2>שלום {userName},</h2>
            <p>קיבלנו בקשה לאיפוס הסיסמה שלך במערכת TMS System.</p>
            <p>לחץ על הכפתור הבא כדי לאפס את הסיסמה שלך:</p>
            
            <div style='text-align: center;'>
                <a href='{resetLink}' class='button'>אפס סיסמה</a>
            </div>

            <div class='warning'>
                <strong>שים לב:</strong>
                <ul style='margin: 10px 0;'>
                    <li>הקישור תקף ל-24 שעות בלבד</li>
                    <li>אם לא ביקשת לאפס את הסיסמה, התעלם מהודעה זו</li>
                    <li>אל תשתף את הקישור עם אף אחד</li>
                </ul>
            </div>

            <p>אם הכפתור לא עובד, העתק והדבק את הקישור הבא בדפדפן:</p>
            <p style='word-break: break-all; color: #0d6efd;'>{resetLink}</p>
        </div>
        <div class='footer'>
            <p>© 2025 TMS System. כל הזכויות שמורות.</p>
            <p>הודעה זו נשלחה אוטומטית, אנא אל תשיב לאימייל זה.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}