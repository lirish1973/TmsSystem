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

                // תיקון השדות בהתאם לטבלה שלך
                PaymentMethods = await _context.PaymentMethods
            .Select(pm => new PaymentMethodSelectViewModel
            {
                PaymentMethodId = pm.ID,  // זה נכון
                PaymentName = pm.METHOD   // זה נכון

            })
            .ToListAsync(),

                Tours = await _context.Tours
            .Select(t => new TourSelectViewModel
            {
                TourId = t.TourId,
                TourName = t.Title ?? string.Empty
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
                    TourId = model.TourId, // הוסף את השורה הזו
                    GuideId = model.GuideId,
                    PaymentId = model.PaymentMethodId,  // השם בטבלה הוא PaymentId, לא PaymentMethodId
                    PaymentMethodId = model.PaymentMethodId, // רק אם הוס
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



        [HttpGet]
        public async Task<IActionResult> ShowOffers(int id)
        {
            try
            {
                var offer = await _context.Offers
                    .Include(o => o.Customer)
                    .Include(o => o.Guide)
                    .Include(o => o.Tour)
                        .ThenInclude(t => t.Schedule)    // טעינת לוח הזמנים
                    .Include(o => o.Tour)
                        .ThenInclude(t => t.Includes)   // טעינת מה כולל הסיור
                    .Include(o => o.Tour)
                        .ThenInclude(t => t.Excludes)   // טעינת מה לא כולל הסיור
                    .FirstOrDefaultAsync(o => o.OfferId == id);

                if (offer == null)
                {
                    TempData["ErrorMessage"] = "ההצעה לא נמצאה";
                    return RedirectToAction(nameof(Index));
                }

                // קבלת פרטי אמצעי התשלום
                PaymentMethod paymentMethod = null;
                if (offer.PaymentMethodId != null && offer.PaymentMethodId > 0)
                {
                    paymentMethod = await _context.PaymentMethods
                        .FirstOrDefaultAsync(pm => pm.ID == offer.PaymentMethodId);
                }

                var viewModel = new ShowOfferViewModel
                {
                    Offer = offer,
                    PaymentMethod = paymentMethod
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בטעינת ההצעה: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        // הוסף את הפונקציות הבאות ל-OffersController הקיים:

        // עריכת הצעה - GET
        public async Task<IActionResult> Edit(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Customer)
                .Include(o => o.Guide)
                .Include(o => o.Tour)
                .FirstOrDefaultAsync(o => o.OfferId == id);

            if (offer == null)
            {
                return NotFound();
            }

            var model = new CreateOfferViewModel
            {
                OfferId = offer.OfferId,
                CustomerId = offer.CustomerId,
                GuideId = offer.GuideId,
                TourId = offer.TourId,
                Participants = offer.Participants,
                TripDate = offer.TripDate,
                TourDate = offer.TourDate,
                PickupLocation = offer.PickupLocation,
                Price = offer.Price,
                TotalPayment = offer.TotalPayment,
                PriceIncludes = offer.PriceIncludes,
                PriceExcludes = offer.PriceExcludes,
                SpecialRequests = offer.SpecialRequests,
                LunchIncluded = offer.LunchIncluded,
                PaymentId = offer.PaymentId,
                PaymentMethodId = offer.PaymentMethodId ?? 0
            };

            await LoadSelectLists(model);
            return View(model);
        }

        // עריכת הצעה - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateOfferViewModel model)
        {
            if (id != model.OfferId)
            {
                return NotFound();
            }

            try
            {
                var offer = await _context.Offers.FindAsync(id);
                if (offer == null)
                {
                    return NotFound();
                }

                // עדכון הפרטים
                offer.CustomerId = model.CustomerId;
                offer.GuideId = model.GuideId;
                offer.TourId = model.TourId;
                offer.Participants = model.Participants;
                offer.TripDate = model.TripDate;
                offer.TourDate = model.TourDate;
                offer.PickupLocation = model.PickupLocation;
                offer.Price = model.Price;
                offer.TotalPayment = model.TotalPayment;
                offer.PriceIncludes = model.PriceIncludes;
                offer.PriceExcludes = model.PriceExcludes;
                offer.SpecialRequests = model.SpecialRequests;
                offer.LunchIncluded = model.LunchIncluded;
                offer.PaymentId = model.PaymentId;
                offer.PaymentMethodId = model.PaymentMethodId;

                _context.Update(offer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "הצעת המחיר עודכנה בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בעדכון הצעת המחיר: {ex.Message}";
                await LoadSelectLists(model);
                return View(model);
            }
        }

        // מחיקת הצעה - GET
        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Customer)
                .Include(o => o.Guide)
                .Include(o => o.Tour)
                .FirstOrDefaultAsync(o => o.OfferId == id);

            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // מחיקת הצעה - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var offer = await _context.Offers.FindAsync(id);
                if (offer != null)
                {
                    _context.Offers.Remove(offer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "הצעת המחיר נמחקה בהצלחה!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה במחיקת הצעת המחיר: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
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

                model.Tours = await _context.Tours
          .Where(t => !string.IsNullOrEmpty(t.Title))
          .Select(t => new TourSelectViewModel
          {
              TourId = t.TourId,
              TourName = t.Title
          })
          .ToListAsync();

                // לוג לבדיקה
                Console.WriteLine($"Loaded {model.Customers.Count} customers");
                Console.WriteLine($"Loaded {model.Guides.Count} guides");
                Console.WriteLine($"Loaded {model.PaymentMethods.Count} payment methods");
                Console.WriteLine($"Loaded {model.Tours.Count} tours"); // הוסף לוג לסיורים
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading select lists: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> TestData()
        {
            var offers = await _context.Offers.ToListAsync();
            var customers = await _context.Customers.ToListAsync();
            var guides = await _context.Guides.ToListAsync();
            var tours = await _context.Tours.ToListAsync();

            var result = new
            {
                OffersCount = offers.Count,
                CustomersCount = customers.Count,
                GuidesCount = guides.Count,
                ToursCount = tours.Count,
                Offers = offers.Select(o => new {
                    o.OfferId,
                    o.CustomerId,
                    o.GuideId,
                    o.TourId,
                    CustomerName = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId)?.FullName
                })
            };

            return Json(result);
        }



        public async Task<IActionResult> Index()
        {
            try
            {
                Console.WriteLine("נכנס לפעולת Index");

                // Debug: ספור כמה רשומות יש בכל טבלה
                var customerCount = await _context.Customers.CountAsync();
                var guideCount = await _context.Guides.CountAsync();
                var tourCount = await _context.Tours.CountAsync();
                var offerCount = await _context.Offers.CountAsync();

                Console.WriteLine($"לקוחות: {customerCount}, מדריכים: {guideCount}, טיולים: {tourCount}, הצעות: {offerCount}");

                if (offerCount == 0)
                {
                    Console.WriteLine("אין הצעות בטבלה!");
                    return View(new List<Offer>());
                }

                var offers = await _context.Offers
          .Include(o => o.Customer)
          .Include(o => o.Guide)
          .Include(o => o.Tour)
          .OrderByDescending(o => o.CreatedAt)
          .ToListAsync();

                Console.WriteLine($"נטענו {offers.Count} הצעות");

                // Debug: הדפס פרטי ההצעה הראשונה
                if (offers.Any())
                {
                    var firstOffer = offers.First();
                    Console.WriteLine($"הצעה ראשונה: ID={firstOffer.OfferId}, Customer={firstOffer.Customer?.FullName}");
                }
                foreach(var offer in offers)
        {
                    if (offer.PaymentMethodId.HasValue)
                    {
                        offer.PaymentMethod = await _context.PaymentMethods
                            .FirstOrDefaultAsync(pm => pm.ID == offer.PaymentMethodId.Value);
                    }
                }

                return View(offers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שגיאה: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"שגיאה בטעינת ההצעות: {ex.Message}";
                return View(new List<Offer>());
            }
                  
        }
    }
}