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



        // GET: /Users/AdminResetPassword/{id}
        [HttpGet]
        public async Task<IActionResult> AdminResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "מזהה משתמש לא תקין.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;

            return View();
        }

        // POST: /Users/AdminResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminResetPassword(string id, string confirm)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "מזהה משתמש לא תקין.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "משתמש לא נמצא.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, "@#$TempPass123");

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"הסיסמה אופסה בהצלחה למשתמש <strong>{user.UserName}</strong>.<br/>הסיסמה החדשה: <code>@#$TempPass123</code>";
                }
                else
                {
                    TempData["ErrorMessage"] = "שגיאה באיפוס הסיסמה:<br/>" +
                        string.Join("<br/>", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"אירעה שגיאה: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
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