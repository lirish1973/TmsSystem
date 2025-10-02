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
                // בדיקות תקינות מפורטות
                var validationErrors = new List<string>();

                // בדיקת לקוח
                if (model.CustomerId <= 0)
                {
                    validationErrors.Add("לא נבחר לקוח");
                }
                else
                {
                    var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == model.CustomerId);
                    if (!customerExists)
                    {
                        validationErrors.Add($"הלקוח עם ID {model.CustomerId} לא קיים במערכת");
                    }
                }

                // בדיקת מדריך
                if (model.GuideId <= 0)
                {
                    validationErrors.Add("לא נבחר מדריך");
                }
                else
                {
                    var guideExists = await _context.Guides.AnyAsync(g => g.GuideId == model.GuideId);
                    if (!guideExists)
                    {
                        validationErrors.Add($"המדריך עם ID {model.GuideId} לא קיים במערכת");
                    }
                }

                // בדיקת אמצעי תשלום
                if (model.PaymentMethodId <= 0)
                {
                    validationErrors.Add("לא נבחר אמצעי תשלום");
                }
                else
                {
                    var paymentExists = await _context.PaymentMethods.AnyAsync(p => p.ID == model.PaymentMethodId);
                    if (!paymentExists)
                    {
                        validationErrors.Add($"אמצעי התשלום עם ID {model.PaymentMethodId} לא קיים במערכת");
                    }
                }

                // בדיקת סיור (אם יש טבלת Tours)
                int tourId = model.TourId > 0 ? model.TourId : 1;
                var tourExists = await _context.Tours.AnyAsync(t => t.TourId == tourId);
                if (!tourExists)
                {
                    validationErrors.Add($"הסיור עם ID {tourId} לא קיים במערכת");
                }

                // בדיקות נוספות
                if (model.TourDate <= DateTime.Today)
                {
                    validationErrors.Add("תאריך הטיול חייב להיות בעתיד");
                }

                if (model.Participants <= 0)
                {
                    validationErrors.Add("מספר המשתתפים חייב להיות גדול מאפס");
                }

                if (model.Price <= 0)
                {
                    validationErrors.Add("המחיר חייב להיות גדול מאפס");
                }

                // הצגת השגיאות אם יש
                if (validationErrors.Any())
                {
                    foreach (var error in validationErrors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    await LoadSelectLists(model);
                    return View(model);
                }

                // יצירת ההצעה
                var offer = new Offer
                {
                    CustomerId = model.CustomerId,
                    GuideId = model.GuideId,
                    TourId = tourId,
                    Participants = model.Participants,
                    TripDate = model.TourDate, // מיפוי לשדה TripDate
                    TourDate = model.TourDate, // מיפוי לשדה TourDate
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

                // בדיקה לפני השמירה
                Console.WriteLine($"CustomerId: {offer.CustomerId}");
                Console.WriteLine($"GuideId: {offer.GuideId}");
                Console.WriteLine($"TourId: {offer.TourId}");
                Console.WriteLine($"PaymentId: {offer.PaymentId}");
                Console.WriteLine($"Participants: {offer.Participants}");
                Console.WriteLine($"Price: {offer.Price}");
                Console.WriteLine($"TotalPayment: {offer.TotalPayment}");

                _context.Offers.Add(offer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "הצעת המחיר נוצרה בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                // שגיאות מסד נתונים מפורטות
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                ModelState.AddModelError("", $"שגיאה בשמירה למסד הנתונים: {innerException}");

                // הדפסה לקונסול לניפוי באגים
                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"InnerException: {innerException}");

                await LoadSelectLists(model);
                return View(model);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"שגיאה כללית: {innerException}");

                Console.WriteLine($"General Exception: {ex.Message}");
                Console.WriteLine($"InnerException: {innerException}");

                await LoadSelectLists(model);
                return View(model);
            }
        }

        private async Task LoadSelectLists(CreateOfferViewModel model)
        {
            try
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

                model.PaymentMethods = await _context.PaymentMethods
                    .Select(pm => new PaymentMethodSelectViewModel
                    {
                        PaymentMethodId = pm.ID,
                        PaymentName = pm.METHOD
                    })
                    .ToListAsync();

                // לוג לבדיקה
                Console.WriteLine($"Loaded {model.Customers.Count} customers");
                Console.WriteLine($"Loaded {model.Guides.Count} guides");
                Console.WriteLine($"Loaded {model.PaymentMethods.Count} payment methods");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading select lists: {ex.Message}");
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var offers = await _context.Offers
                    .Include(o => o.Customer)
                    .Include(o => o.GuideId) // השם במודל
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