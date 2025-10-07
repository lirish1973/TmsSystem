using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;

namespace TmsSystem.Controllers
{
   

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = dbContext;
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

        // הוספת פעולת Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                Address = user.Address,
                CompanyName = user.CompanyName,
                BirthDate = user.BirthDate,
                RegistrationDate = user.RegistrationDate
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
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

                return RedirectToAction("Profile");
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




        [HttpGet]
        public ActionResult EditProfile(int id)
        {
            // שליפה מה־DB
            var user = _context.Users.FirstOrDefault(u => u.Id == id.ToString());
            if (user == null)
            {
                return NotFound(); // אם המשתמש לא נמצא
            }

            // בניית המודל
            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Phone = user.PhoneNumber,
                CompanyName = user.CompanyName,
                BirthDate = user.BirthDate
            };
            return View(model); // שלח את המודל ל־View
        }

        [HttpPost]
        public ActionResult EditProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // שליפת המשתמש מה־DB
                var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
                if (user == null)
                {
                    return NotFound(); // אם המשתמש לא נמצא
                }

                // עדכון פרטי המשתמש
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.Phone;
                user.CompanyName = model.CompanyName;
                user.BirthDate = model.BirthDate;

              

                return RedirectToAction("ProfileUpdated");
            }

            return View(model); // החזרת המודל עם שגיאות אם יש
        }

        [HttpGet]
        public IActionResult ProfileUpdated()
        {
            ViewData["Message"] = "הפרופיל עודכן בהצלחה.";
            return View();
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