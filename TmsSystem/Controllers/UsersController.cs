using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;

namespace TmsSystem.Controllers
{
    [Authorize(Roles = "Admin")] // רק מנהלים יכולים לגשת
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            

        }

        // GET: /Users
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: /Users/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // עדכון שדות
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.CompanyName = model.CompanyName;
            user.BirthDate = model.BirthDate;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


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

            return View("UserProfile", model);  // ← שינוי כאן
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserProfile()  // שינוי מ-Profile ל-UserProfile
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
                return View("UserProfile", model);  // ← שינוי כאן

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
                        return View("UserProfile", model);  // ← שינוי כאן
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

            return View("UserProfile", model);  // ← שינוי כאן
        }



        [HttpGet]
        public IActionResult ProfileUpdated()
        {
            ViewData["Message"] = "הפרופיל עודכן בהצלחה.";
            return View();
        }



        // POST: /Users/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}