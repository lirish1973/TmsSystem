using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;   // לוודא שה־namespace נכון אצלך
using TmsSystem.Models;

namespace TmsSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            // סופרים משתמשים
            var totalUsers = _userManager.Users.Count();
            ViewBag.TotalUsers = totalUsers;

            // סופרים לקוחות
            var totalCustomers = _context.Customers.Count();
            ViewBag.TotalCustomers = totalCustomers;


            var TotalTours = _context.Tours.Count();
            ViewBag.TotalTours = TotalTours;


            return View();
        }
    }
}
