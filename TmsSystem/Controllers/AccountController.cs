using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;

namespace TmsSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ===================== REGISTER =====================
        [HttpGet]
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
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // לוג מפורט של שגיאות
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error Code: {error.Code} | Description: {error.Description}");
            }
            ViewData["ErrorMessage"] = string.Join("<br>", result.Errors.Select(e => $"{e.Code}: {e.Description}"));

            return View(model);
        }

        // ===================== LOGIN =====================
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






        //=========================password Reset=========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא.";
                return RedirectToAction("Index");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, "@#$TempPass123");

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"הסיסמה אופסה בהצלחה ל- '#$TempPass123'.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("<br>", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Index");
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
    }
}
