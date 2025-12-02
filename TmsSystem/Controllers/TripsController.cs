using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.Services;

namespace TmsSystem.Controllers
{
    [Authorize]
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly TripEmailSender _tripEmailSender; // הוסף את זה!

        public TripsController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            TripEmailSender tripEmailSender) // הוסף פרמטר!
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _tripEmailSender = tripEmailSender; // אתחול!
        }

        // GET: Trips
        public async Task<IActionResult> Index(string filter)
        {
            var tripsQuery = _context.Trips
                .Include(t => t.TripDays)
                .Include(t => t.Guide)  // 👈 להוסיף את זה!
                .AsQueryable();

            if (filter == "active")
            {
                tripsQuery = tripsQuery.Where(t => t.IsActive);
            }

            var trips = await tripsQuery
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.Filter = filter;
            return View(trips);
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                Console.WriteLine("❌ Details: id is null");
                return NotFound();
            }

            Console.WriteLine($"🔍 Loading trip details for ID: {id}");

            var trip = await _context.Trips
                .Include(t => t.TripDays.OrderBy(d => d.DayNumber))
                .Include(t => t.Guide) // 👈 חשוב מאוד!
                .FirstOrDefaultAsync(m => m.TripId == id);

            if (trip == null)
            {
                Console.WriteLine($"❌ Trip {id} not found");
                return NotFound();
            }

            Console.WriteLine($"✅ Trip loaded: {trip.Title}");
            Console.WriteLine($"👨‍✈️ GuideId in DB: {trip.GuideId}");
            Console.WriteLine($"👨‍✈️ Guide object loaded: {trip.Guide != null}");

            if (trip.Guide != null)
            {
                Console.WriteLine($"👨‍✈️ Guide Name: {trip.Guide.GuideName}");
            }
            else if (trip.GuideId.HasValue)
            {
                Console.WriteLine($"⚠️ WARNING: GuideId={trip.GuideId} but Guide object is NULL!  Check database relationship.");
            }
            else
            {
                Console.WriteLine("ℹ️ No guide assigned to this trip");
            }

            return View(trip);
        }


        // GET: Trips/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // טעינת רשימת מדריכים
                await LoadGuidesDropdown();

                Console.WriteLine("✅ Create page loaded");

                // יצירת מודל ריק
                var trip = new Trip
                {
                    NumberOfDays = 7,
                    IsActive = true
                };

                return View(trip);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading Create page: {ex.Message}");

                ViewBag.Guides = new SelectList(new List<object>(), "GuideId", "GuideName");

                return View(new Trip { NumberOfDays = 7, IsActive = true });
            }
        }


        // POST: Trips/Create
        // POST: Trips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trip trip, IFormCollection form)
        {
            try
            {
                Console.WriteLine($"📝 Creating trip: {trip.Title}");

                if (!ModelState.IsValid)
                {
                    await LoadGuidesDropdown(trip.GuideId);
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"❌ Validation Error: {error.ErrorMessage}");
                    }
                    return View(trip);
                }

                // בדיקת תקינות מדריך
                if (trip.GuideId.HasValue)
                {
                    var guideExists = await _context.Guides
                        .AnyAsync(g => g.GuideId == trip.GuideId.Value && g.IsActive);

                    if (!guideExists)
                    {
                        ModelState.AddModelError("GuideId", "המדריך שנבחר לא קיים או לא פעיל");
                        await LoadGuidesDropdown(trip.GuideId);
                        return View(trip);
                    }
                    Console.WriteLine($"👨‍✈️ Guide assigned: {trip.GuideId}");
                }

                trip.CreatedAt = DateTime.Now;

                // ==================== טיפול בתמונות ====================
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "trips");
                Directory.CreateDirectory(uploadsFolder);

                var tripDaysList = trip.TripDays?.OrderBy(d => d.DayNumber).ToList() ?? new List<TripDay>();

                for (int i = 0; i < tripDaysList.Count; i++)
                {
                    // חיפוש קובץ עבור יום זה בשם dayImages_0, dayImages_1 וכו'
                    var fileKey = $"dayImages_{i}";
                    var files = form.Files.GetFiles(fileKey);

                    if (files != null && files.Count > 0)
                    {
                        var file = files[0];
                        if (file != null && file.Length > 0)
                        {
                            try
                            {
                                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                tripDaysList[i].ImagePath = $"/uploads/trips/{fileName}";
                                Console.WriteLine($"✅ Image uploaded for day {i + 1}: {fileName}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"⚠️ Error uploading image for day {i + 1}: {ex.Message}");
                            }
                        }
                    }
                }

                trip.TripDays = tripDaysList;

                _context.Add(trip);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Trip created successfully: ID={trip.TripId}");
                TempData["SuccessMessage"] = $"הטיול '{trip.Title}' נוצר בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating trip: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");

                await LoadGuidesDropdown(trip.GuideId);

                ModelState.AddModelError("", $"שגיאה ביצירת הטיול: {ex.Message}");
                return View(trip);
            }
        }




        private async Task LoadGuidesDropdown(int? selectedGuideId = null)
        {
            try
            {
                const int PLACEHOLDER_GUIDE_ID = 9; // 👈 המדריך הפיקטיבי

                var guides = await _context.Guides
                    .Where(g => g.IsActive && g.GuideId != PLACEHOLDER_GUIDE_ID) // 👈 הסתרה
                    .OrderBy(g => g.GuideName)
                    .Select(g => new { g.GuideId, g.GuideName })
                    .ToListAsync();

                ViewBag.Guides = new SelectList(guides, "GuideId", "GuideName", selectedGuideId);

                Console.WriteLine($"✅ Loaded {guides.Count} guides for dropdown");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading guides: {ex.Message}");
                ViewBag.Guides = new SelectList(new List<object>(), "GuideId", "GuideName");
            }
        }




        // POST: /trips/{id}/send-email
        [HttpPost("/trips/{id}/send-email")]
        public async Task<IActionResult> SendTripEmail(int id, [FromForm] string email, [FromForm] string customerName = null)
        {
            try
            {
                var trip = await _context.Trips
                    .Include(t => t.TripDays)
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (trip == null)
                    return Json(new { success = false, message = "הטיול לא נמצא" });

                if (string.IsNullOrWhiteSpace(email))
                    return Json(new { success = false, message = "כתובת אימייל נדרשת" });

                // 🔧 תיקון: קריאה נכונה למתודה
                var result = await _tripEmailSender.SendTripProposalAsync(trip, email, customerName);

                if (result.Success)
                {
                    return Json(new
                    {
                        success = true,
                        sentTo = result.SentTo,
                        subject = result.Subject,
                        sentAt = result.SentAt.ToString("dd/MM/yyyy HH:mm:ss"),
                        provider = result.Provider,
                        tripId = result.TripId
                    });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SendTripEmail] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { success = false, message = $"שגיאה: {ex.Message}" });
            }
        }


        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.TripDays.OrderBy(d => d.DayNumber))
                .Include(t => t.Guide)
                .FirstOrDefaultAsync(t => t.TripId == id);

            if (trip == null)
            {
                return NotFound();
            }

            // טעינת מדריכים
            await LoadGuidesDropdown(trip.GuideId);

            Console.WriteLine($"✅ Editing trip: {trip.Title}");
            Console.WriteLine($"👨‍✈️ Current guide: {trip.Guide?.GuideName ?? "None"}");

            return View(trip);
        }



        // POST: Trips/Edit/5
        // POST: Trips/Edit/5
        // POST: Trips/Edit/5
        // POST: Trips/Edit/5
        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trip trip, IFormCollection form)
        {
            if (id != trip.TripId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadGuidesDropdown(trip.GuideId);

                var vmTemp = await _context.Trips
                    .Include(t => t.TripDays)
                    .Include(t => t.Guide)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (vmTemp?.TripDays != null)
                    trip.TripDays = vmTemp.TripDays.OrderBy(d => d.DayNumber).ToList();

                return View(trip);
            }

            try
            {
                var existingTrip = await _context.Trips
                    .Include(t => t.TripDays)
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (existingTrip == null)
                    return NotFound();

                Console.WriteLine($"[Edit] Starting edit for Trip {id}, Title: {trip.Title}");
                Console.WriteLine($"👨‍✈️ Guide ID from form: {trip.GuideId}");
                Console.WriteLine($"👨‍✈️ Existing trip GuideId before update: {existingTrip.GuideId}");

                // בדיקת תקינות מדריך
                if (trip.GuideId.HasValue)
                {
                    var guideExists = await _context.Guides
                        .AnyAsync(g => g.GuideId == trip.GuideId.Value && g.IsActive);

                    if (!guideExists)
                    {
                        ModelState.AddModelError("GuideId", "המדריך שנבחר לא קיים או לא פעיל");
                        await LoadGuidesDropdown(trip.GuideId);

                        var vmTemp = await _context.Trips
                            .Include(t => t.TripDays)
                            .Include(t => t.Guide)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(t => t.TripId == id);

                        if (vmTemp?.TripDays != null)
                            trip.TripDays = vmTemp.TripDays.OrderBy(d => d.DayNumber).ToList();

                        return View(trip);
                    }
                    Console.WriteLine($"👨‍✈️ Guide validated: {trip.GuideId}");
                }

                // ==================== עדכון שדות בסיסיים ====================
                existingTrip.Title = trip.Title;
                existingTrip.Description = trip.Description;
                existingTrip.IsActive = trip.IsActive;
                existingTrip.NumberOfDays = trip.NumberOfDays;
                existingTrip.PricePerPerson = trip.PricePerPerson;
                existingTrip.PriceDescription = trip.PriceDescription;
                existingTrip.Includes = trip.Includes;
                existingTrip.Excludes = trip.Excludes;
                existingTrip.FlightDetails = trip.FlightDetails;
                existingTrip.GuideId = trip.GuideId;

                Console.WriteLine($"👨‍✈️ Existing trip GuideId after manual assignment: {existingTrip.GuideId}");

                _context.Entry(existingTrip).Property(t => t.GuideId).IsModified = true;

                Console.WriteLine($"👨‍✈️ GuideId marked as modified: {_context.Entry(existingTrip).Property(t => t.GuideId).IsModified}");

                // ==================== טיפול ב-TripDays ====================
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath ?? "", "uploads", "trips");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var existingDayIds = new HashSet<int>();

                // עדכון ימים קיימים
                if (trip.TripDays != null && trip.TripDays.Any())
                {
                    foreach (var day in trip.TripDays)
                    {
                        var existingDay = existingTrip.TripDays.FirstOrDefault(d => d.TripDayId == day.TripDayId);

                        if (existingDay != null)
                        {
                            // עדכון שדות טקסט של היום
                            existingDay.Title = day.Title;
                            existingDay.Location = day.Location;
                            existingDay.Description = day.Description;
                            existingDay.DisplayOrder = day.DisplayOrder;
                            existingDay.DayNumber = day.DayNumber;

                            Console.WriteLine($"✏️ Updated TripDay text {day.TripDayId} (Day #{day.DayNumber}): {day.Title}");

                            existingDayIds.Add(day.TripDayId);
                            _context.Update(existingDay);
                        }
                    }
                }

                // ==================== טיפול בתמונות חדשות ====================
                // 🔑 חיפוש לפי displayOrder (0-based) אבל כל יום בטופס מגיע בשם dayImage_0, dayImage_1 וכו'
                var orderedDays = existingTrip.TripDays.OrderBy(d => d.DayNumber).ToList();

                for (int displayIndex = 0; displayIndex < orderedDays.Count; displayIndex++)
                {
                    try
                    {
                        // חיפוש קובץ עבור יום זה בשם dayImage_0, dayImage_1 וכו'
                        var fileKey = $"dayImage_{displayIndex}";
                        var files = form.Files.GetFiles(fileKey);

                        Console.WriteLine($"🔍 Looking for image {fileKey}...  Found: {(files?.Count > 0 ? files.Count : 0)} files");

                        // בדיקה: האם המשתמש בחר קובץ חדש?
                        if (files != null && files.Count > 0)
                        {
                            var file = files[0];

                            // ✅ אם יש קובץ ולא ריק - העלה אותו
                            if (file != null && file.Length > 0)
                            {
                                var existingDay = orderedDays[displayIndex];

                                Console.WriteLine($"📸 Processing image for day index {displayIndex}, TripDayId: {existingDay.TripDayId}");

                                // ❌ מחיקת תמונה ישנה רק אם הוספנו חדשה
                                if (!string.IsNullOrEmpty(existingDay.ImagePath))
                                {
                                    var oldImagePath = Path.Combine(
                                        _webHostEnvironment.WebRootPath ?? "",
                                        existingDay.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                                    );

                                    try
                                    {
                                        if (System.IO.File.Exists(oldImagePath))
                                        {
                                            System.IO.File.Delete(oldImagePath);
                                            Console.WriteLine($"🗑️ Deleted old image for day {displayIndex + 1}: {oldImagePath}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"⚠️ Could not delete old image: {ex.Message}");
                                    }
                                }

                                // הוספת תמונה חדשה
                                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                existingDay.ImagePath = $"/uploads/trips/{fileName}";
                                Console.WriteLine($"✅ Updated image for day {displayIndex + 1}: {fileName}");
                                _context.Update(existingDay);
                            }
                            else
                            {
                                Console.WriteLine($"ℹ️ No image file for day index {displayIndex} (empty file), keeping existing image");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"ℹ️ No new image for day index {displayIndex}, keeping existing image");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error processing image for day index {displayIndex}: {ex.Message}");
                    }
                }

                // ==================== מחיקת ימים שנמחקו ====================
                var daysToDelete = existingTrip.TripDays.Where(d => !existingDayIds.Contains(d.TripDayId)).ToList();
                foreach (var dayToDelete in daysToDelete)
                {
                    // מחיקת תמונה אם קיימת
                    if (!string.IsNullOrEmpty(dayToDelete.ImagePath))
                    {
                        var imagePath = Path.Combine(
                            _webHostEnvironment.WebRootPath ?? "",
                            dayToDelete.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                        );

                        try
                        {
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                                Console.WriteLine($"🗑️ Deleted image for removed day {dayToDelete.TripDayId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Could not delete image: {ex.Message}");
                        }
                    }

                    _context.TripDays.Remove(dayToDelete);
                    Console.WriteLine($"🗑️ Removed TripDay {dayToDelete.TripDayId}: {dayToDelete.Title}");
                }

                // ==================== שמירת שינויים ====================
                Console.WriteLine("[Edit] Saving changes to database...");
                Console.WriteLine($"👨‍✈️ Final GuideId value before save: {existingTrip.GuideId}");

                var changesCount = await _context.SaveChangesAsync();

                Console.WriteLine($"[Edit] Changes saved successfully!  {changesCount} records affected");
                Console.WriteLine($"👨‍✈️ GuideId after save: {existingTrip.GuideId}");

                TempData["SuccessMessage"] = $"הטיול '{existingTrip.Title}' עודכן בהצלחה! ";
                return RedirectToAction(nameof(Details), new { id = existingTrip.TripId });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"[Edit] Database error: {ex.Message}");
                Console.WriteLine($"[Edit] Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine(ex.StackTrace);

                await LoadGuidesDropdown(trip.GuideId);

                ModelState.AddModelError("", $"שגיאה בעדכון הנתונים: {ex.InnerException?.Message ?? ex.Message}");

                var vmTemp = await _context.Trips
                    .Include(t => t.TripDays)
                    .Include(t => t.Guide)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (vmTemp?.TripDays != null)
                    trip.TripDays = vmTemp.TripDays.OrderBy(d => d.DayNumber).ToList();

                return View(trip);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Edit] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                await LoadGuidesDropdown(trip.GuideId);

                ModelState.AddModelError("", $"שגיאה: {ex.Message}");

                var vmTemp = await _context.Trips
                    .Include(t => t.TripDays)
                    .Include(t => t.Guide)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (vmTemp?.TripDays != null)
                    trip.TripDays = vmTemp.TripDays.OrderBy(d => d.DayNumber).ToList();

                return View(trip);
            }
        }






        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.TripDays)
                .FirstOrDefaultAsync(m => m.TripId == id);

            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Console.WriteLine($"🗑️ Starting delete process for Trip ID: {id}");

                var trip = await _context.Trips
                    .Include(t => t.TripDays)
                    .Include(t => t.TripOffers) // 👈 טעינת הצעות המחיר
                        .ThenInclude(o => o.Customer) // 👈 לצורך הצגת שם הלקוח בלוג
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (trip == null)
                {
                    Console.WriteLine($"❌ Trip {id} not found");
                    TempData["ErrorMessage"] = "הטיול לא נמצא";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"✅ Trip found: {trip.Title}");
                Console.WriteLine($"📊 Trip has {trip.TripDays?.Count ?? 0} days");
                Console.WriteLine($"📊 Trip has {trip.TripOffers?.Count ?? 0} trip offers");

                // 🔍 בדיקה אם יש הצעות מחיר מקושרות
                if (trip.TripOffers != null && trip.TripOffers.Any())
                {
                    Console.WriteLine($"⚠️ Found {trip.TripOffers.Count} trip offers linked to this trip");

                    // מחיקת כל ההצעות קודם
                    foreach (var offer in trip.TripOffers.ToList())
                    {
                        Console.WriteLine($"🗑️ Deleting TripOffer {offer.TripOfferId} - {offer.OfferNumber} ({offer.Customer?.DisplayName ?? "Unknown"})");
                        _context.TripOffers.Remove(offer);
                    }

                    Console.WriteLine("✅ All trip offers marked for deletion");
                }

                // 🖼️ מחיקת תמונות של ימי הטיול
                if (trip.TripDays != null && trip.TripDays.Any())
                {
                    Console.WriteLine($"🖼️ Processing {trip.TripDays.Count} day images.. .");

                    foreach (var day in trip.TripDays)
                    {
                        if (!string.IsNullOrEmpty(day.ImagePath))
                        {
                            var imagePath = Path.Combine(
                                _webHostEnvironment.WebRootPath ?? "",
                                day.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                            );

                            try
                            {
                                if (System.IO.File.Exists(imagePath))
                                {
                                    System.IO.File.Delete(imagePath);
                                    Console.WriteLine($"🗑️ Deleted image: {imagePath}");
                                }
                                else
                                {
                                    Console.WriteLine($"⚠️ Image not found: {imagePath}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"⚠️ Could not delete image {imagePath}: {ex.Message}");
                            }
                        }
                    }
                }

                // 🗑️ מחיקת הטיול (ימי הטיול יימחקו אוטומטית עם cascade)
                Console.WriteLine($"🗑️ Removing trip from context...");
                _context.Trips.Remove(trip);

                Console.WriteLine("💾 Saving changes to database...");
                var affectedRows = await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Trip deleted successfully! {affectedRows} rows affected");

                TempData["SuccessMessage"] = $"הטיול '{trip.Title}' נמחק בהצלחה! ";

                if (trip.TripOffers != null && trip.TripOffers.Any())
                {
                    TempData["InfoMessage"] = $"נמחקו גם {trip.TripOffers.Count} הצעות מחיר שהיו מקושרות לטיול";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"❌ Database error deleting trip {id}: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine(ex.StackTrace);

                TempData["ErrorMessage"] = $"שגיאה במחיקת הטיול: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error deleting trip {id}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                TempData["ErrorMessage"] = $"שגיאה במחיקת הטיול: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
    }
}