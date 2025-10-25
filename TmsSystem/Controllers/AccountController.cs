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
            IEmailService emailService,      // ← הוסף
            IConfiguration config)            // ← הוסף
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = dbContext;
            _emailSender = emailSender;
            _emailService = emailService;     // ← הוסף
            _config = config;                 // ← הוסף
        }



        // ===================== REGISTER =====================
        [HttpGet]
        [Authorize(Roles = "Admin")] // רק מנהלים יכולים ליצור משתמשים חדשים
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
                    // אם זה הרשמה רגילה, מחברים את המשתמש
                    await _signInManager.SignInAsync(user, isPersistent: false);
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

        // ===================== LOGIN ===================== (ללא שינוי)
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // חיפוש המשתמש לפי Email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "משתמש לא נמצא");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToLocal(returnUrl);

            if (result.IsLockedOut)
                ModelState.AddModelError("", "החשבון נעול. צור קשר עם המנהל.");
            else if (result.IsNotAllowed)
                ModelState.AddModelError("", "כניסה לא אפשרית. אנא אמת את החשבון.");
            else
                ModelState.AddModelError("", "כניסה נכשלה. בדוק את הפרטים.");

            return View(model);
        }



        // GET: ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            // לא מגלים אם המשתמש קיים או לא - מסיבות אבטחה
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                ViewData["SuccessMessage"] = "אם כתובת האימייל קיימת במערכת, נשלח אליך קישור לאיפוס סיסמה.";
                return View();
            }

            // יצירת טוקן לאיפוס סיסמה
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // יצירת קישור לאיפוס סיסמה
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { code = code, email = model.Email },
                protocol: Request.Scheme);

            // יצירת תוכן HTML לאימייל
            var htmlBody = GenerateResetPasswordEmailHtml(user.UserName ?? user.Email, callbackUrl);
            var plainTextBody = $"שלום {user.UserName ?? user.Email},\n\n" +
                               $"קיבלנו בקשה לאיפוס הסיסמה שלך.\n\n" +
                               $"לחץ על הקישור הבא לאיפוס הסיסמה:\n{callbackUrl}\n\n" +
                               $"הקישור תקף ל-24 שעות.\n\n" +
                               $"אם לא ביקשת לאפס את הסיסמה, התעלם מהודעה זו.\n\n" +
                               $"בברכה,\nצוות TMS System";

            try
            {
                // שליחת מייל דרך IEmailService (SendGrid)
                await _emailService.SendHtmlAsync(
                    toEmail: model.Email,
                    subject: "איפוס סיסמה - מערכת TMS",
                    htmlBody: htmlBody,
                    plainTextBody: plainTextBody
                );

                ViewData["SuccessMessage"] = "קישור לאיפוס סיסמה נשלח לאימייל שלך.";
            }
            catch (Exception ex)
            {
                // במקרה של שגיאה, נסה עם IEmailSender (גיבוי)
                try
                {
                    await _emailSender.SendEmailAsync(
                        model.Email,
                        "איפוס סיסמה - מערכת TMS",
                        $"אנא אפס את הסיסמה שלך על ידי <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>לחיצה כאן</a>.");

                    ViewData["SuccessMessage"] = "אם כתובת האימייל קיימת במערכת, נשלח אליך קישור לאיפוס סיסמה.";
                }
                catch
                {
                    ViewData["ErrorMessage"] = "אירעה שגיאה בשליחת האימייל. אנא נסה שוב מאוחר יותר.";
                    Console.WriteLine($"Error sending reset password email: {ex.Message}");
                }
            }

            return View();
        }


        // פונקציה ליצירת HTML לאימייל
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


        // ===================== LOGOUT =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ===================== עזר פרטי =====================
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        // ===================== Password Reset =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string id)
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
    }
}