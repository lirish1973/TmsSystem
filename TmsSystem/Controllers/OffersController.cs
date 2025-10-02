using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;
using static TmsSystem.ViewModels.CreateOfferViewModel;

namespace TmsSystem.Controllers
{

    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OffersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            var model = new CreateOfferViewModel
            {
                Guides = await _context.Guides
                    .Select(g => new GuideSelectViewModel
                    {
                        GuideId = g.GuideId,
                        GuideName = g.GuideName ?? string.Empty
                    })
                    .ToListAsync(),

                Customers = await _context.Customers
                    .Select(c => new CustomerSelectViewModel
                    {
                        CustomerId = c.CustomerId,
                        DisplayName = (c.FullName ?? c.CustomerName ?? string.Empty) + $" ({c.Phone})"
                    })
                    .ToListAsync(),

                // תיקון כאן - PaymentMethods במקום PaymentsMethod
                PaymentMethods = await _context.PaymentMethods
                    .Select(pm => new PaymentMethodSelectViewModel
                    {
                        PaymentMethodId = pm.ID,        // שימוש ב-ID (אותיות גדולות)
                        PaymentName = pm.METHOD         // שימוש ב-METHOD (אותיות גדולות)
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .Include(o => o.Customer)
                .Include(o => o.GuideId)
                .Include(o => o.Tour)
                .ToListAsync();

            return View(offers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfferViewModel model)
        {
            try
            {
                // בדיקת תקינות בסיסית
                if (model.TourDate <= DateTime.Today)
                {
                    ModelState.AddModelError("TourDate", "תאריך הטיול חייב להיות בעתיד");
                }

                if (model.Participants <= 0)
                {
                    ModelState.AddModelError("Participants", "מספר המשתתפים חייב להיות גדול מאפס");
                }

                if (!ModelState.IsValid)
                {
                    await LoadSelectLists(model);
                    return View(model);
                }

                // יצירת ההצעה
                var offer = new Offer
                {
                    CustomerId = model.CustomerId,
                    GuideId = model.GuideId,
                    TourId = model.TourId > 0 ? model.TourId : 1,
                    Participants = model.Participants,
                    TripDate = model.TourDate,  // העתק לשני השדות
                    TourDate = model.TourDate,  // כפי שמוגדר בטבלה
                    PickupLocation = model.PickupLocation ?? "",
                    Price = model.Price,
                    TotalPayment = model.Price * model.Participants,
                    PriceIncludes = model.PriceIncludes ?? "",
                    PriceExcludes = model.PriceExcludes ?? "",
                    SpecialRequests = model.SpecialRequests ?? "",
                    LunchIncluded = model.LunchIncluded,
                    PaymentId = model.PaymentMethodId,
                    CreatedAt = DateTime.Now
                };

                _context.Offers.Add(offer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "הצעת המחיר נוצרה בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // רישום שגיאה מפורט
                var innerException = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"שגיאה בשמירת הנתונים: {innerException}");

                await LoadSelectLists(model);
                return View(model);
            }
        }

        private async Task LoadSelectLists(CreateOfferViewModel model)
        {
            model.Customers = await _context.Customers
                .Where(c => c.CustomerId > 0)
                .Select(c => new CustomerSelectViewModel
                {
                    CustomerId = c.CustomerId,
                    DisplayName = !string.IsNullOrEmpty(c.FullName) ?
                        $"{c.FullName} ({c.Phone})" :
                        $"{c.CustomerName} ({c.Phone})"
                })
                .ToListAsync();

            model.Guides = await _context.Guides
                .Where(g => !string.IsNullOrEmpty(g.GuideName))
                .Select(g => new GuideSelectViewModel
                {
                    GuideId = g.GuideId,
                    GuideName = g.GuideName
                })
                .ToListAsync();

            model.Tours = await _context.Tours
                .Select(t => new TourSelectViewModel
                {
                    TourId = t.TourId,
                    TourName = t.Title ?? "סיור לא מוגדר"
                })
                .ToListAsync();

            model.PaymentMethods = await _context.PaymentMethods
                .Select(pm => new PaymentMethodSelectViewModel
                {
                    PaymentMethodId = pm.ID,
                    PaymentName = pm.METHOD
                })
                .ToListAsync();
        }
    }
}