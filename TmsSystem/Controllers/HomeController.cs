using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TmsSystem.Data;
using TmsSystem.Models;

namespace TmsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // העברת נתונים סטטיסטיים לדף
            ViewBag.TotalUsers = _userManager.Users.Count();
            ViewBag.TotalCustomers = _context.Customers.Count();
            ViewBag.TotalTours = _context.Tours.Count();

            // בדיקת תפקיד המשתמש הנוכחי
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var roles = await _userManager.GetRolesAsync(currentUser);
                    ViewBag.UserRoles = roles;
                    ViewBag.IsAdmin = roles.Contains("Admin");
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}