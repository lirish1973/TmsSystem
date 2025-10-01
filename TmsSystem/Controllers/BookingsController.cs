using Microsoft.AspNetCore.Mvc;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;

namespace TmsSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            // אפשר למלא dropdowns של לקוחות/סיורים
            ViewBag.Customers = _context.Customers.ToList();
            ViewBag.Tours = _context.Tours.ToList();
            return View(new CreateBookingViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateBookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _context.Customers.ToList();
                ViewBag.Tours = _context.Tours.ToList();
                return View(model);
            }

            var booking = new TourBooking
            {
                CustomerId = model.CustomerId,
                TourId = model.TourId,
                GroupName = model.GroupName,
                BookerName = model.BookerName,
                ParticipantsCount = model.ParticipantsCount,
                PickupLocation = model.PickupLocation,
                TourDate = model.TourDate,
                GuideName = model.GuideName,
                SpecialRequests = model.SpecialRequests,
                IncludesLunch = model.IncludesLunch
            };

            _context.TourBookings.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("Index", "Bookings");
        }
    }
}
