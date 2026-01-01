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

        public async Task<IActionResult> Index()
        {
            try
            {
                // משתמשים (רק למנהלים)
                if (User.IsInRole("Admin"))
                {
                    ViewBag.TotalUsers = await _context.Users.CountAsync();
                }

                // לקוחות
                ViewBag.TotalCustomers = await _context.Customers.CountAsync(c => !c.IsDeleted);

                // 🆕 מדריכים
                ViewBag.TotalGuides = await _context.Guides.CountAsync(g => g.IsActive);

                // סיורים
                ViewBag.TotalTours = await _context.Tours.CountAsync();

                // 🆕 טיולים
                ViewBag.TotalTrips = await _context.Trips.CountAsync(t => t.IsActive);

                // הצעות מחיר קרובות (בשבועיים הקרובים)
                var twoWeeksFromNow = DateTime.Now.AddDays(14);
                ViewBag.NearOffers = await _context.Offers
                    .Where(o => o.TourDate >= DateTime.Now && o.TourDate <= twoWeeksFromNow)
                    .CountAsync();

                // סטטיסטיקות נוספות
                ViewBag.TodayTrips = 0;
                ViewBag.ApprovedOffers = 0;
                ViewBag.PendingOffers = 0;

                return View();
            }
            catch (Exception ex)
            {
                // Log the error
                return View("Error");
            }
        }
    }
}
