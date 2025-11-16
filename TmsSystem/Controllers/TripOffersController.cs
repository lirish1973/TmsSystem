using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TmsSystem.Controllers
{
    public class TripOffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripOffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TripOffers
        public async Task<IActionResult> Index()
        {
            var offers = await _context.TripOffers
                .Include(to => to.Customer)
                .Include(to => to.Trip)
                .Include(to => to.PaymentMethod)
                .OrderByDescending(to => to.OfferDate)
                .ToListAsync();

            return View(offers);
        }

        // GET: TripOffers/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateTripOfferViewModel
            {
                Customers = await _context.Customers
                    .OrderBy(c => c.FullName)
                    .ToListAsync(),

                Trips = await _context.Trips
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Title)
                    .ToListAsync(),

                PaymentMethods = await _context.PaymentMethods
                    .OrderBy(p => p.PaymentName)
                    .ToListAsync(),

                DepartureDate = DateTime.Today.AddDays(30), // ברירת מחדל: בעוד חודש
                Participants = 2
            };

            return View(viewModel);
        }

        // POST: TripOffers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTripOfferViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // טען מחדש את הרשימות
                    model.Customers = await _context.Customers.ToListAsync();
                    model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                    model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                    return View(model);
                }

                // חישוב תאריך חזרה אוטומטי אם לא הוזן
                if (!model.ReturnDate.HasValue)
                {
                    var trip = await _context.Trips.FindAsync(model.TripId);
                    if (trip != null)
                    {
                        model.ReturnDate = model.DepartureDate.AddDays(trip.NumberOfDays);
                    }
                }

                // חישוב מחיר כולל
                decimal totalPrice = model.PricePerPerson * model.Participants;

                // הוספת תוספת חדר יחיד
                if (model.SingleRoomSupplement.HasValue && model.SingleRooms > 0)
                {
                    totalPrice += model.SingleRoomSupplement.Value * model.SingleRooms;
                }

                // הוספת ביטוח
                if (model.InsuranceIncluded && model.InsurancePrice.HasValue)
                {
                    totalPrice += model.InsurancePrice.Value * model.Participants;
                }

                // יצירת מספר הצעת מחיר ייחודי
                var offerNumber = $"TRIP-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

                var tripOffer = new TripOffer
                {
                    CustomerId = model.CustomerId,
                    TripId = model.TripId,
                    OfferNumber = offerNumber,
                    OfferDate = DateTime.Now,
                    Participants = model.Participants,
                    DepartureDate = model.DepartureDate,
                    ReturnDate = model.ReturnDate,
                    PricePerPerson = model.PricePerPerson,
                    SingleRoomSupplement = model.SingleRoomSupplement,
                    SingleRooms = model.SingleRooms,
                    TotalPrice = totalPrice,
                    PaymentMethodId = model.PaymentMethodId,
                    PaymentInstallments = model.PaymentInstallments,
                    FlightIncluded = model.FlightIncluded,
                    FlightDetails = model.FlightDetails,
                    InsuranceIncluded = model.InsuranceIncluded,
                    InsurancePrice = model.InsurancePrice,
                    SpecialRequests = model.SpecialRequests,
                    AdditionalNotes = model.AdditionalNotes,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                _context.TripOffers.Add(tripOffer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"הצעת מחיר '{offerNumber}' נוצרה בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"שגיאה ביצירת הצעת מחיר: {ex.Message}");
                model.Customers = await _context.Customers.ToListAsync();
                model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                return View(model);
            }
        }

        // GET: TripOffers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripOffer = await _context.TripOffers
                .Include(to => to.Customer)
                .Include(to => to.Trip)
                    .ThenInclude(t => t.TripDays)
                .Include(to => to.PaymentMethod)
                .FirstOrDefaultAsync(m => m.TripOfferId == id);

            if (tripOffer == null)
            {
                return NotFound();
            }

            return View(tripOffer);
        }

        // GET API: TripOffers/GetTripDetails/5
        [HttpGet]
        public async Task<IActionResult> GetTripDetails(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            return Json(new
            {
                tripId = trip.TripId,
                title = trip.Title,
                numberOfDays = trip.NumberOfDays,
                pricePerPerson = trip.PricePerPerson,
                priceDescription = trip.PriceDescription,
                includes = trip.Includes,
                excludes = trip.Excludes,
                flightDetails = trip.FlightDetails
            });
        }
    }
}