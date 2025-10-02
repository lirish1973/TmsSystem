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

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfferViewModel model)
        {
            try
            {
                // בדיקות תקינות
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

                // יצירת ההצעה - שמירה לטבלה עם השדות הנכונים
                var offer = new Offer
                {
                    CustomerId = model.CustomerId,
                    GuideId = model.GuideId,
                    TourId = model.TourId > 0 ? model.TourId : 1, // ברירת מחדל
                    Participants = model.Participants,
                    TripDate = model.TourDate, // שמירה לשדה TripDate בטבלה
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
                // הצגת שגיאה מפורטת
                var fullError = ex.InnerException?.Message ?? ex.Message;

                // אם זה שגיאת foreign key
                if (fullError.Contains("foreign key") || fullError.Contains("FOREIGN KEY"))
                {
                    ModelState.AddModelError("", "שגיאה: אחד מהערכים שנבחרו לא קיים במערכת. אנא בדוק את הבחירות.");
                }
                else
                {
                    ModelState.AddModelError("", $"שגיאה בשמירת הנתונים: {fullError}");
                }

                await LoadSelectLists(model);
                return View(model);
            }
        }

        private async Task LoadSelectLists(CreateOfferViewModel model)
        {
            try
            {
                // לקוחות
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

                // מדריכים - שימוש בשם הטבלה הנכון
                model.Guides = await _context.Guides
                    .Where(g => !string.IsNullOrEmpty(g.GuideName))
                    .Select(g => new GuideSelectViewModel
                    {
                        GuideId = g.GuideId,
                        GuideName = g.GuideName
                    })
                    .ToListAsync();

                // אמצעי תשלום
                model.PaymentMethods = await _context.PaymentMethods
                    .Select(pm => new PaymentMethodSelectViewModel
                    {
                        PaymentMethodId = pm.ID,
                        PaymentName = pm.METHOD
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // אם יש בעיה בטעינת הנתונים
                ModelState.AddModelError("", $"שגיאה בטעינת נתוני הטופס: {ex.Message}");
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var offers = await _context.Offers
                    .Include(o => o.Customer)
                    .Include(o => o.GuideName) // השם במודל
                    .Include(o => o.Tour)
                    .ToListAsync();

                return View(offers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בטעינת ההצעות: {ex.Message}";
                return View(new List<Offer>());
            }
        }
    }
}