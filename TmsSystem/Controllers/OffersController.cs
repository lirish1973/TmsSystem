using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.Services;
using TmsSystem.ViewModels;
using static TmsSystem.ViewModels.CreateOfferViewModel;

namespace TmsSystem.Controllers
{
    public partial class OffersController : Controller
    {
        // הכרזת השדות — זה מה שהיה חסר
        private readonly IPdfService _pdfService;
        private readonly OfferEmailSender _offerEmailSender;
        private readonly ApplicationDbContext _context;



        public OffersController(ApplicationDbContext context, IPdfService pdfService, OfferEmailSender offerEmailSender)
        {
            _context = context;
            _pdfService = pdfService;
            _offerEmailSender = offerEmailSender;
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

                PaymentMethods = await _context.PaymentMethods
                    .Select(pm => new PaymentMethodSelectViewModel
                    {
                        PaymentMethodId = pm.ID,
                        PaymentName = pm.METHOD
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




        [HttpGet("/offers/{id}/preview-html")]
        public async Task<IActionResult> PreviewHtml(int id)
        {
            var model = await LoadOfferViewModelAsync(id); // תממש אצלך
            var html = await _pdfService.GenerateOfferHtmlAsync(model);
            return Content(html, "text/html; charset=utf-8");
        }

        [HttpPost("/offers/{id}/send-email")]
        public async Task<IActionResult> SendEmail(int id, [FromForm] string toEmail = null)
        {
            var model = await LoadOfferViewModelAsync(id);
            
            if (model == null)
            {
                return NotFound("ההצעה לא נמצאה.");
            }

            // If toEmail is not provided in form, use customer email
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                toEmail = model.Offer?.Customer?.Email;
            }

            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest("לא נמצא אימייל ללקוח.");

            await _offerEmailSender.SendOfferEmailAsync(model, toEmail);
            return Ok(new { ok = true, sentTo = toEmail });
        }


        private async Task<ShowOfferViewModel> LoadOfferViewModelAsync(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Customer)
                .Include(o => o.Guide)
                .Include(o => o.Tour)
                .ThenInclude(t => t.Schedule)
                .Include(o => o.Tour)
                .ThenInclude(t => t.Includes)
                .Include(o => o.Tour)
                .ThenInclude(t => t.Excludes)
                .FirstOrDefaultAsync(o => o.OfferId == id);

            if (offer == null)
            {
                return null;
            }

            PaymentMethod paymentMethod = null;
            if (offer.PaymentMethodId != null && offer.PaymentMethodId > 0)
            {
                paymentMethod = await _context.PaymentMethods
                    .FirstOrDefaultAsync(pm => pm.ID == offer.PaymentMethodId);
            }

            return new ShowOfferViewModel
            {
                Offer = offer,
                PaymentMethod = paymentMethod
            };
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfferViewModel model)
        {
            try
            {
                var validationErrors = new List<string>();

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

                int tourId = model.TourId > 0 ? model.TourId : 1;
                var tourExists = await _context.Tours.AnyAsync(t => t.TourId == tourId);
                if (!tourExists)
                {
                    validationErrors.Add($"הסיור עם ID {tourId} לא קיים במערכת");
                }

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

                if (validationErrors.Any())
                {
                    foreach (var error in validationErrors)
                    {
                        ModelState.AddModelError("", error);
                    }

                    await LoadSelectLists(model);
                    return View(model);
                }

                var offer = new Offer
                {
                    CustomerId = model.CustomerId,
                    TourId = model.TourId,
                    GuideId = model.GuideId,
                    PaymentMethodId = model.PaymentMethodId,
                    Participants = model.Participants,
                    TripDate = model.TourDate,
                    TourDate = model.TourDate,
                    PickupLocation = model.PickupLocation ?? "",
                    Price = model.Price,
                    TotalPayment = model.Price * model.Participants,
                    PriceIncludes = model.PriceIncludes ?? "",
                    PriceExcludes = model.PriceExcludes ?? "",
                    SpecialRequests = model.SpecialRequests ?? "",
                    LunchIncluded = model.LunchIncluded,
                    CreatedAt = DateTime.Now
                };

                _context.Offers.Add(offer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "הצעת המחיר נוצרה בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                ModelState.AddModelError("", $"שגיאה בשמירה למסד הנתונים: {innerException}");

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
                    .ThenInclude(t => t.Schedule)
                    .Include(o => o.Tour)
                    .ThenInclude(t => t.Includes)
                    .Include(o => o.Tour)
                    .ThenInclude(t => t.Excludes)
                    .FirstOrDefaultAsync(o => o.OfferId == id);

                if (offer == null)
                {
                    TempData["ErrorMessage"] = "ההצעה לא נמצאה";
                    return RedirectToAction(nameof(Index));
                }

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
                PaymentMethodId = offer.PaymentMethodId ?? 0
            };

            await LoadSelectLists(model);
            return View(model);
        }

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

        // GET: Offers/Delete/5 - מתודה אחת בלבד!
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers
                .Include(o => o.Customer)
                .Include(o => o.Guide)
                .Include(o => o.Tour)
                .Include(o => o.PaymentMethod)
                .FirstOrDefaultAsync(m => m.OfferId == id);

            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offers/Delete/5 - גרסה מתוקנת עם טיפול בשגיאות
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
                    TempData["SuccessMessage"] = "ההצעה נמחקה בהצלחה!";
                }
                else
                {
                    TempData["ErrorMessage"] = "ההצעה לא נמצאה במערכת.";
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message?.Contains("foreign key") == true)
                {
                    TempData["ErrorMessage"] = "לא ניתן למחוק את ההצעה מכיוון שהיא מקושרת לנתונים אחרים במערכת.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"שגיאה במחיקת ההצעה: {ex.Message}";
                }

                Console.WriteLine($"Delete error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה כללית: {ex.Message}";
                Console.WriteLine($"General delete error: {ex.Message}");
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
                        DisplayName = !string.IsNullOrEmpty(c.FullName)
                            ? $"{c.FullName} ({c.Phone})"
                            : $"{c.CustomerName} ({c.Phone})"
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

                Console.WriteLine($"Loaded {model.Customers.Count} customers");
                Console.WriteLine($"Loaded {model.Guides.Count} guides");
                Console.WriteLine($"Loaded {model.PaymentMethods.Count} payment methods");
                Console.WriteLine($"Loaded {model.Tours.Count} tours");
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
                Offers = offers.Select(o => new
                {
                    o.OfferId,
                    o.CustomerId,
                    o.GuideId,
                    o.TourId,
                    CustomerName = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId)?.FullName
                })
            };

            return Json(result);
        }

        // הוסף מתודה חדשה
        // הוסף את המתודה החדשה


        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            try
            {
                var offer = await _context.Offers
                    .Include(o => o.Customer)
                    .Include(o => o.Guide)
                    .Include(o => o.Tour)
                    .Include(o => o.PaymentMethod)
                    .FirstOrDefaultAsync(o => o.OfferId == id);


               

                if (offer == null)
                {
                    TempData["ErrorMessage"] = "ההצעה לא נמצאה";
                    return RedirectToAction(nameof(Index));
                }

                PaymentMethod paymentMethod = null;
                if (offer.PaymentMethodId.HasValue)
                {
                    paymentMethod = await _context.PaymentMethods
                        .FirstOrDefaultAsync(pm => pm.ID == offer.PaymentMethodId.Value);
                }

                var viewModel = new ShowOfferViewModel
                {
                    Offer = offer,
                    PaymentMethod = paymentMethod
                };



                var pdfBytes = await _pdfService.GenerateOfferPdfAsync(viewModel);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    TempData["ErrorMessage"] = "יצירת קובץ PDF נכשלה.";
                    return RedirectToAction(nameof(ShowOffers), new { id });
                }

                var fileName = $"הצעת_מחיר_{offer.OfferId}.pdf";

                // ✅ פתרון הבעיה - הוספת Headers שיכפו הורדה
                Response.Headers.Add("Content-Disposition", $"attachment; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");
                Response.Headers.Add("Expires", "0");

                return File(pdfBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה ביצירת PDF: {ex.Message}";
                return RedirectToAction(nameof(ShowOffers), new { id });
            }
        }







        public async Task<IActionResult> Index(string filter)
        {
            try
            {




        var offerCount = await _context.Offers.CountAsync();
                if (offerCount == 0)
                {
                    ViewBag.PageTitle = filter == "near" ? "הצעות מחיר קרובות" : "ניהול הצעות מחיר";
                    ViewBag.ShowNearOnly = filter == "near";
                    return View(new List<Offer>());
                }

                var offersQuery = _context.Offers.AsQueryable();

                // אם יש פילטר של "near" - מציג רק הצעות מחיר קרובות
                if (filter == "near")
                {
                    var twoWeeksFromNow = DateTime.Today.AddDays(14);
                    offersQuery = offersQuery.Where(o => o.TourDate >= DateTime.Today && o.TourDate <= twoWeeksFromNow);
                    ViewBag.PageTitle = "הצעות מחיר קרובות";
                    ViewBag.ShowNearOnly = true;
                }
                else
                {
                    ViewBag.PageTitle = "ניהול הצעות מחיר";
                    ViewBag.ShowNearOnly = false;
                }

                var offers = await offersQuery
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                foreach (var offer in offers)
                {
                    try
                    {
                        offer.Customer = await _context.Customers.FindAsync(offer.CustomerId);
                        offer.Guide = await _context.Guides.FindAsync(offer.GuideId);
                        offer.Tour = await _context.Tours.FindAsync(offer.TourId);

                        if (offer.PaymentMethodId.HasValue)
                        {
                            offer.PaymentMethod = await _context.PaymentMethods
                                .FirstOrDefaultAsync(pm => pm.ID == offer.PaymentMethodId.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"שגיאה בטעינת יחסים להצעה {offer.OfferId}: {ex.Message}");
                    }
                }

                return View(offers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"שגיאה בטעינת ההצעות: {ex.Message}";
                ViewBag.PageTitle = filter == "near" ? "הצעות מחיר קרובות" : "ניהול הצעות מחיר";
                ViewBag.ShowNearOnly = filter == "near";
                return View(new List<Offer>());
            }
        }


    }

    }
    
