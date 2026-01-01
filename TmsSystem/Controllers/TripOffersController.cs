using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.Services;
using TmsSystem.ViewModels;
using System.IO;

namespace TmsSystem.Controllers
{
    [Authorize]
    public class TripOffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TripOfferEmailSender _tripOfferEmailSender;

        public TripOffersController(
            ApplicationDbContext context,
            TripOfferEmailSender tripOfferEmailSender)
        {
            _context = context;
            _tripOfferEmailSender = tripOfferEmailSender;
        }

        // GET: TripOffers
        public async Task<IActionResult> Index()
        {
            try
            {
                Console.WriteLine("🔍 Starting TripOffers/Index");
                var totalInDb = await _context.TripOffers.CountAsync();
                Console.WriteLine($"📊 Total TripOffers in DB: {totalInDb}");


                var offers = await _context.TripOffers
                    .Include(to => to.Customer)
                    .Include(to => to.Trip)
                        .ThenInclude(t => t.TripDays)
                    .Include(to => to.PaymentMethod)
                    .OrderByDescending(to => to.OfferDate)
                    .ToListAsync();

                Console.WriteLine($"✅ Loaded {offers.Count} trip offers");

                // 🆕 הדפס את הנתונים
                foreach (var offer in offers)
                {
                    Console.WriteLine($"  📋 Offer: {offer.OfferNumber} - {offer.Trip?.Title ?? "No Title"}");
                }

                Console.WriteLine("🎯 Returning View with Model");

                return View(offers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FULL ERROR: {ex}");
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<TripOffer>());

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"🔥 InnerException: {ex.InnerException.Message}");
                }

                Console.WriteLine($"📍 StackTrace: {ex.StackTrace}");

                TempData["ErrorMessage"] = $"שגיאה: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }


        // 🗑️ DELETE: TripOffers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine($"🗑️ Starting delete process for TripOffer ID: {id}");

                var tripOffer = await _context.TripOffers
                    .Include(to => to.Customer)
                    .Include(to => to.Trip)
                    .FirstOrDefaultAsync(to => to.TripOfferId == id);

                if (tripOffer == null)
                {
                    Console.WriteLine($"❌ TripOffer {id} not found");
                    return Json(new { success = false, message = "הצעת המחיר לא נמצאה" });
                }

                Console.WriteLine($"✅ Found TripOffer: {tripOffer.OfferNumber}");
                Console.WriteLine($"📊 Deleting offer for customer: {tripOffer.Customer?.DisplayName}");

                // מחיקת ההצעה
                _context.TripOffers.Remove(tripOffer);

                Console.WriteLine("💾 Saving changes to database...");
                var affectedRows = await _context.SaveChangesAsync();

                Console.WriteLine($"✅ TripOffer deleted successfully!  {affectedRows} rows affected");

                return Json(new
                {
                    success = true,
                    message = $"הצעת המחיר '{tripOffer.OfferNumber}' נמחקה בהצלחה!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting TripOffer {id}: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    message = $"שגיאה במחיקת הצעת המחיר: {ex.Message}"
                });
            }
        }

        // GET: TripOffers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var tripOffer = await _context.TripOffers
                    .Include(to => to.Customer)
                    .Include(to => to.Trip.TripDays)    // ✅ טעינה ישירה
                    .Include(to => to.Trip.Guide)        // ✅ טעינה ישירה
                    .Include(to => to.PaymentMethod)
                    .FirstOrDefaultAsync(m => m.TripOfferId == id);

                if (tripOffer == null)
                {
                    return NotFound();
                }

                return View(tripOffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in TripOffers/Details: {ex.Message}");
                Console.WriteLine($"📍 InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"📍 StackTrace: {ex.StackTrace}");

                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }




        // GET: TripOffers/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateTripOfferViewModel
            {
                Customers = await _context.Customers
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.FullName)
                    .ToListAsync(),

                Trips = await _context.Trips
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Title)
                    .ToListAsync(),

                PaymentMethods = await _context.PaymentMethods
                    .OrderBy(p => p.PaymentName)
                    .ToListAsync(),

                DepartureDate = DateTime.Today.AddDays(30),
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
                    model.Customers = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
                    model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                    model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                    return View(model);
                }

                if (!model.ReturnDate.HasValue)
                {
                    var trip = await _context.Trips.FindAsync(model.TripId);
                    if (trip != null)
                    {
                        model.ReturnDate = model.DepartureDate.AddDays(trip.NumberOfDays);
                    }
                }

                decimal totalPrice = model.PricePerPerson * model.Participants;

                if (model.SingleRoomSupplement.HasValue && model.SingleRooms > 0)
                {
                    totalPrice += model.SingleRoomSupplement.Value * model.SingleRooms;
                }

                if (model.InsuranceIncluded && model.InsurancePrice.HasValue)
                {
                    totalPrice += model.InsurancePrice.Value * model.Participants;
                }

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
            catch (DbUpdateException dbEx)
            {
                // הדפס את השגיאה המלאה
                Console.WriteLine($"❌ DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"🔥 InnerException: {dbEx.InnerException?.Message}");
                Console.WriteLine($"🔥 Inner-Inner: {dbEx.InnerException?.InnerException?.Message}");

                string errorMsg = dbEx.InnerException?.InnerException?.Message
                               ?? dbEx.InnerException?.Message
                               ?? dbEx.Message;

                ModelState.AddModelError("", $"שגיאה בשמירה: {errorMsg}");
                model.Customers = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
                model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                Console.WriteLine($"🔥 InnerException: {ex.InnerException?.Message}");

                ModelState.AddModelError("", $"שגיאה: {ex.InnerException?.Message ?? ex.Message}");
                model.Customers = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
                model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                return View(model);
            }
        }

        // GET: TripOffers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var tripOffer = await _context.TripOffers
                    .Include(to => to.Customer)
                    .Include(to => to.Trip)
                    .Include(to => to.PaymentMethod)
                    .FirstOrDefaultAsync(to => to.TripOfferId == id);

                if (tripOffer == null)
                {
                    return NotFound();
                }

                // המרה ל-ViewModel
                var viewModel = new CreateTripOfferViewModel
                {
                    CustomerId = tripOffer.CustomerId,
                    TripId = tripOffer.TripId,
                    Participants = tripOffer.Participants,
                    DepartureDate = tripOffer.DepartureDate,
                    ReturnDate = tripOffer.ReturnDate,
                    PricePerPerson = tripOffer.PricePerPerson,
                    SingleRoomSupplement = tripOffer.SingleRoomSupplement,
                    SingleRooms = tripOffer.SingleRooms,
                    PaymentMethodId = tripOffer.PaymentMethodId,
                    PaymentInstallments = tripOffer.PaymentInstallments,
                    FlightIncluded = tripOffer.FlightIncluded,
                    FlightDetails = tripOffer.FlightDetails,
                    InsuranceIncluded = tripOffer.InsuranceIncluded,
                    InsurancePrice = tripOffer.InsurancePrice,
                    SpecialRequests = tripOffer.SpecialRequests,
                    AdditionalNotes = tripOffer.AdditionalNotes,

                    // טעינת הרשימות
                    Customers = await _context.Customers.Where(c => !c.IsDeleted).OrderBy(c => c.FullName).ToListAsync(),
                    Trips = await _context.Trips.Where(t => t.IsActive).OrderBy(t => t.Title).ToListAsync(),
                    PaymentMethods = await _context.PaymentMethods.OrderBy(p => p.PaymentName).ToListAsync()
                };

                ViewBag.TripOfferId = id;
                ViewBag.OfferNumber = tripOffer.OfferNumber;
                ViewBag.CurrentStatus = tripOffer.Status;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Edit GET: {ex.Message}");
                TempData["ErrorMessage"] = $"שגיאה בטעינת ההצעה: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TripOffers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateTripOfferViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Customers = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
                    model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                    model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                    return View(model);
                }

                var tripOffer = await _context.TripOffers.FindAsync(id);

                if (tripOffer == null)
                {
                    return NotFound();
                }

                // חישוב מחיר כולל
                decimal totalPrice = model.PricePerPerson * model.Participants;

                if (model.SingleRoomSupplement.HasValue && model.SingleRooms > 0)
                {
                    totalPrice += model.SingleRoomSupplement.Value * model.SingleRooms;
                }

                if (model.InsuranceIncluded && model.InsurancePrice.HasValue)
                {
                    totalPrice += model.InsurancePrice.Value * model.Participants;
                }

                // עדכון השדות
                tripOffer.CustomerId = model.CustomerId;
                tripOffer.TripId = model.TripId;
                tripOffer.Participants = model.Participants;
                tripOffer.DepartureDate = model.DepartureDate;
                tripOffer.ReturnDate = model.ReturnDate;
                tripOffer.PricePerPerson = model.PricePerPerson;
                tripOffer.SingleRoomSupplement = model.SingleRoomSupplement;
                tripOffer.SingleRooms = model.SingleRooms;
                tripOffer.TotalPrice = totalPrice;
                tripOffer.PaymentMethodId = model.PaymentMethodId;
                tripOffer.PaymentInstallments = model.PaymentInstallments;
                tripOffer.FlightIncluded = model.FlightIncluded;
                tripOffer.FlightDetails = model.FlightDetails;
                tripOffer.InsuranceIncluded = model.InsuranceIncluded;
                tripOffer.InsurancePrice = model.InsurancePrice;
                tripOffer.SpecialRequests = model.SpecialRequests;
                tripOffer.AdditionalNotes = model.AdditionalNotes;
                tripOffer.UpdatedAt = DateTime.Now;

                _context.Update(tripOffer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"הצעת מחיר '{tripOffer.OfferNumber}' עודכנה בהצלחה!";
                return RedirectToAction(nameof(Details), new { id = tripOffer.TripOfferId });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"🔥 InnerException: {dbEx.InnerException?.Message}");

                string errorMsg = dbEx.InnerException?.InnerException?.Message
                               ?? dbEx.InnerException?.Message
                               ?? dbEx.Message;

                ModelState.AddModelError("", $"שגיאה בעדכון: {errorMsg}");
                model.Customers = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
                model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                ModelState.AddModelError("", $"שגיאה: {ex.Message}");
                model.Customers = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
                model.Trips = await _context.Trips.Where(t => t.IsActive).ToListAsync();
                model.PaymentMethods = await _context.PaymentMethods.ToListAsync();
                return View(model);
            }
        }

        // POST: TripOffers/UpdateStatus/5
        // POST: TripOffers/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] string status)
        {
            try
            {
                Console.WriteLine($"🔍 UpdateStatus called - ID: {id}, Status: {status}");

                var tripOffer = await _context.TripOffers.FindAsync(id);

                if (tripOffer == null)
                {
                    Console.WriteLine($"❌ TripOffer {id} not found");
                    return Json(new { success = false, message = "הצעת המחיר לא נמצאה" });
                }

                // בדיקת תקינות הסטטוס
                var validStatuses = new[] { "Pending", "Approved", "Rejected", "Cancelled" };
                if (!validStatuses.Contains(status))
                {
                    Console.WriteLine($"❌ Invalid status: {status}");
                    return Json(new { success = false, message = "סטטוס לא תקין" });
                }

                string oldStatus = tripOffer.Status;
                tripOffer.Status = status;
                tripOffer.UpdatedAt = DateTime.Now;

                _context.Update(tripOffer);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Status updated from {oldStatus} to {status}");

                var statusText = status switch
                {
                    "Pending" => "ממתין",
                    "Approved" => "מאושר",
                    "Rejected" => "נדחה",
                    "Cancelled" => "בוטל",
                    _ => status
                };

                return Json(new
                {
                    success = true,
                    message = $"הסטטוס עודכן ל'{statusText}' בהצלחה",
                    newStatus = status,
                    statusText = statusText
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating status: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return Json(new { success = false, message = $"שגיאה: {ex.Message}" });
            }
        }










        // POST: TripOffers/SendEmail/5
        [HttpPost("/TripOffers/SendEmail/{id}")]
        public async Task<IActionResult> SendEmail(int id)
        {
            try
            {
                var offer = await _context.TripOffers
                    .Include(o => o.Customer)
                    .Include(o => o.Trip)
                        .ThenInclude(t => t.TripDays)
                    .Include(o => o.PaymentMethod)
                    .FirstOrDefaultAsync(o => o.TripOfferId == id);

                if (offer == null)
                    return Json(new { success = false, message = "הצעת המחיר לא נמצאה" });

                if (string.IsNullOrWhiteSpace(offer.Customer?.Email))
                    return Json(new { success = false, message = "לא נמצאה כתובת אימייל ללקוח" });

                // ✅ קריאה למתודה הנכונה ב-TripOfferEmailSender
                var result = await _tripOfferEmailSender.SendTripOfferEmailAsync(offer);

                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        sentTo = result.SentTo,
                        subject = result.Subject,
                        sentAt = result.SentAt.ToString("dd/MM/yyyy HH:mm:ss"),
                        provider = result.Provider,
                        customerName = result.CustomerName,
                        offerNumber = offer.OfferNumber
                    });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SendTripOfferEmail] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { success = false, message = $"שגיאה: {ex.Message}" });
            }
        }

        // 🆕 POST: TripOffers/SendEmailWithAttachments/5
        [HttpPost]
        [Route("TripOffers/SendEmailWithAttachments/{id}")]
        public async Task<IActionResult> SendEmailWithAttachments(int id, [FromBody] SendTripEmailRequest request)
        {
            Console.WriteLine($"=== SendEmailWithAttachments START === ID: {id}");

            try
            {
                Console.WriteLine("Step 1: Loading trip offer from database...");

                var tripOffer = await _context.TripOffers
                    .Include(to => to.Customer)
                    .Include(to => to.Trip)
                        .ThenInclude(t => t.TripDays)
                    .Include(to => to.PaymentMethod)
                    .FirstOrDefaultAsync(to => to.TripOfferId == id);

                if (tripOffer == null)
                {
                    Console.WriteLine($"ERROR: Trip offer {id} not found");
                    return Json(new { success = false, message = "הצעת הטיול לא נמצאה" });
                }

                Console.WriteLine($"Step 2: Trip offer found: {tripOffer.OfferNumber}");

                string email = request?.Email ?? tripOffer.Customer?.Email;

                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("ERROR: No email address");
                    return Json(new { success = false, message = "כתובת אימייל נדרשת" });
                }

                Console.WriteLine($"Step 3: Sending email to {email}");
                Console.WriteLine($"Attachments count: {request?.Attachments?.Count ?? 0}");

                // המרת attachments ל-EmailAttachment
                var emailAttachments = request?.Attachments?.Select(a => new EmailAttachment
                {
                    FileName = a.FileName,
                    Base64Content = a.Base64Content,
                    MimeType = a.MimeType
                }).ToList();

                Console.WriteLine("Step 4: Calling email sender...");

                // ✅ קריאה למתודה עם inline images
                var result = await _tripOfferEmailSender.SendTripOfferEmailWithImagesAsync(
                    tripOffer,
                    email,
                    emailAttachments);

                Console.WriteLine($"Step 5: Email result - Success: {result.Success}");

                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        sentTo = result.SentTo,
                        subject = result.Subject,
                        sentAt = result.SentAt.ToString("dd/MM/yyyy HH:mm:ss"),
                        provider = result.Provider,
                        tripOfferId = result.TripOfferId,
                        customerName = result.CustomerName,
                        messageId = result.MessageId ?? ""
                    });
                }
                else
                {
                    Console.WriteLine($"Email failed: {result.ErrorMessage}");
                    return Json(new
                    {
                        success = false,
                        message = result.ErrorMessage
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== EXCEPTION ===");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    message = $"שגיאה בשליחת המייל: {ex.Message}"
                });
            }
        }
    }

    // Model classes לקבלת הנתונים מה-JavaScript
    public class SendTripEmailRequest
    {
        public string Email { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
    }

    public class AttachmentDto
    {
        public string FileName { get; set; }
        public string Base64Content { get; set; }
        public string MimeType { get; set; }
    }
}