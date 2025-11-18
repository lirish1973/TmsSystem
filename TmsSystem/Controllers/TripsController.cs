using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;

namespace TmsSystem.Controllers
{
    [Authorize]
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TripsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Trips
        public async Task<IActionResult> Index(string filter)
        {
            var tripsQuery = _context.Trips.Include(t => t.TripDays).AsQueryable();

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

        // GET: Trips/Create
        public IActionResult Create()
        {
            var trip = new Trip
            {
                NumberOfDays = 5,
                IsActive = true,
                TripDays = new List<TripDay>()
            };

            // יצירת 5 ימים ברירת מחדל
            for (int i = 1; i <= 5; i++)
            {
                trip.TripDays.Add(new TripDay
                {
                    DayNumber = i,
                    DisplayOrder = i,
                    Title = $"יום {i}"
                });
            }

            return View(trip);
        }

        // POST: Trips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trip trip)
        {
            try
            {
                // הסרת Validation Errors עבור Navigation Properties
                ModelState.Remove("TripDays");

                // בדיקת נתונים בסיסיים
                if (string.IsNullOrWhiteSpace(trip.Title))
                {
                    ModelState.AddModelError("Title", "שם הטיול הוא שדה חובה");
                    return View(trip);
                }

                if (trip.NumberOfDays < 5 || trip.NumberOfDays > 12)
                {
                    ModelState.AddModelError("NumberOfDays", "מספר הימים חייב להיות בין 5 ל-12");
                    return View(trip);
                }

                // יצירת תיקיית uploads אם לא קיימת
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "trips");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // קבלת הימים מה-Request
                var tripDaysList = new List<TripDay>();
                for (int i = 0; i < trip.NumberOfDays; i++)
                {
                    var title = Request.Form[$"TripDays[{i}].Title"].ToString();
                    var location = Request.Form[$"TripDays[{i}].Location"].ToString();
                    var description = Request.Form[$"TripDays[{i}].Description"].ToString();
                    var dayNumber = int.Parse(Request.Form[$"TripDays[{i}].DayNumber"].ToString());

                    var tripDay = new TripDay
                    {
                        DayNumber = dayNumber,
                        Title = title,
                        Location = location,
                        Description = description,
                        DisplayOrder = dayNumber
                    };

                    // טיפול בתמונה של אותו יום - שימוש בשם ייחודי
                    var imageFile = Request.Form.Files[$"dayImage_{i}"];
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        tripDay.ImagePath = "/uploads/trips/" + uniqueFileName;
                    }

                    tripDaysList.Add(tripDay);
                }

                // יצירת הטיול עם כל הפרטים
                var newTrip = new Trip
                {
                    Title = trip.Title,
                    Description = trip.Description,
                    NumberOfDays = trip.NumberOfDays,
                    IsActive = trip.IsActive,
                    CreatedAt = DateTime.Now,

                    // פרטי מחיר - חדש!
                    PricePerPerson = trip.PricePerPerson,
                    PriceDescription = trip.PriceDescription,
                    Includes = trip.Includes,
                    Excludes = trip.Excludes,
                    FlightDetails = trip.FlightDetails,

                    // ימי הטיול
                    TripDays = tripDaysList
                };

                _context.Trips.Add(newTrip);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"הטיול '{newTrip.Title}' נוצר בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"שגיאה בשמירת הטיול: {ex.Message}");
                return View(trip);
            }
        }




        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // בדיקה אם הטיול קיים
                var tripExists = await _context.Trips.AnyAsync(t => t.TripId == id);
                Console.WriteLine($"Trip {id} exists: {tripExists}");

                // טעינת הטיול עם הימים
                var trip = await _context.Trips
                    .Include(t => t.TripDays)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (trip == null)
                {
                    Console.WriteLine($"Trip {id} not found");
                    return NotFound();
                }

                Console.WriteLine($"Trip loaded: {trip.Title}");
                Console.WriteLine($"TripDays count: {trip.TripDays?.Count ?? 0}");

                // בדיקה ישירה במסד הנתונים
                var daysCount = await _context.TripDays.CountAsync(td => td.TripId == id);
                Console.WriteLine($"Days in database: {daysCount}");

                if (trip.TripDays == null || !trip.TripDays.Any())
                {
                    // טעינה ידנית של הימים אם לא נטענו
                    var days = await _context.TripDays
                        .Where(td => td.TripId == id)
                        .OrderBy(td => td.DayNumber)
                        .ToListAsync();

                    Console.WriteLine($"Manually loaded days: {days.Count}");
                    trip.TripDays = days;
                }
                else
                {
                    // מיון הימים
                    trip.TripDays = trip.TripDays.OrderBy(d => d.DayNumber).ToList();
                }

                Console.WriteLine($"Final TripDays count: {trip.TripDays.Count}");

                return View(trip);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Edit: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }



        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trip trip)
        {
            if (id != trip.TripId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // טען מחדש את הימים אם יש שגיאת ולידציה
                var vmTemp = await _context.Trips
                    .Include(t => t.TripDays)
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

                // עדכון שדות כלליים
                existingTrip.Title = trip.Title;
                existingTrip.Description = trip.Description;
                existingTrip.IsActive = trip.IsActive;
                existingTrip.NumberOfDays = trip.NumberOfDays;
                existingTrip.PricePerPerson = trip.PricePerPerson;
                existingTrip.PriceDescription = trip.PriceDescription;
                existingTrip.Includes = trip.Includes;
                existingTrip.Excludes = trip.Excludes;
                existingTrip.FlightDetails = trip.FlightDetails;

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath ?? "", "uploads", "trips");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var existingDayIds = new HashSet<int>();

                // עבור על הימים מה-Form (לפי NumberOfDays)
                for (int i = 0; i < trip.NumberOfDays; i++)
                {
                    // קריאת ערכים מה-Request.Form
                    var tripDayIdStr = Request.Form[$"TripDays[{i}].TripDayId"].FirstOrDefault() ?? "0";
                    int.TryParse(tripDayIdStr, out var tripDayId);

                    var dayNumberStr = Request.Form[$"TripDays[{i}].DayNumber"].FirstOrDefault() ?? (i + 1).ToString();
                    int.TryParse(dayNumberStr, out var dayNumber);

                    var displayOrderStr = Request.Form[$"TripDays[{i}].DisplayOrder"].FirstOrDefault() ?? dayNumber.ToString();
                    int.TryParse(displayOrderStr, out var displayOrder);

                    var title = Request.Form[$"TripDays[{i}].Title"].FirstOrDefault() ?? $"יום {dayNumber}";
                    var location = Request.Form[$"TripDays[{i}].Location"].FirstOrDefault() ?? string.Empty;
                    var description = Request.Form[$"TripDays[{i}].Description"].FirstOrDefault() ?? string.Empty;
                    var existingImagePath = Request.Form[$"TripDays[{i}].ImagePath"].FirstOrDefault() ?? string.Empty;

                    Console.WriteLine($"[Edit] Processing day {i}: TripDayId={tripDayId}, DayNumber={dayNumber}, Title='{title}'");

                    TripDay tripDay = null;

                    if (tripDayId > 0)
                    {
                        // עדכון יום קיים
                        tripDay = existingTrip.TripDays.FirstOrDefault(d => d.TripDayId == tripDayId);

                        if (tripDay != null)
                        {
                            Console.WriteLine($"[Edit] Updating existing day {tripDayId}: '{tripDay.Title}' -> '{title}'");

                            tripDay.Title = title;
                            tripDay.Location = location;
                            tripDay.Description = description;
                            tripDay.DayNumber = dayNumber;
                            tripDay.DisplayOrder = displayOrder;

                            _context.Entry(tripDay).State = EntityState.Modified;
                            existingDayIds.Add(tripDayId);
                        }
                        else
                        {
                            Console.WriteLine($"[Edit] Warning: TripDay {tripDayId} not found in existingTrip.TripDays");
                            continue;
                        }
                    }
                    else
                    {
                        // יום חדש
                        tripDay = new TripDay
                        {
                            TripId = existingTrip.TripId,
                            DayNumber = dayNumber,
                            Title = title,
                            Location = location,
                            Description = description,
                            DisplayOrder = displayOrder,
                            ImagePath = existingImagePath
                        };

                        existingTrip.TripDays.Add(tripDay);
                        _context.Entry(tripDay).State = EntityState.Added;
                        Console.WriteLine($"[Edit] Added new day {dayNumber}: '{title}'");
                    }

                    // טיפול בתמונה - שים לב לשם השדה: dayImage_i (ללא סוגריים מסולסלים)
                    var imageFile = Request.Form.Files[$"dayImage_{i}"];

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        Console.WriteLine($"[Edit] Processing image for day {i}: {imageFile.FileName} ({imageFile.Length} bytes)");

                        // מחיקת תמונה ישנה אם קיימת
                        if (!string.IsNullOrEmpty(tripDay.ImagePath))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath ?? "",
                                tripDay.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                            try
                            {
                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                    Console.WriteLine($"[Edit] Deleted old image: {oldImagePath}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Edit] Could not delete old image: {ex.Message}");
                            }
                        }

                        // שמירת תמונה חדשה
                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fs);
                        }

                        tripDay.ImagePath = "/uploads/trips/" + uniqueFileName;

                        if (tripDay.TripDayId > 0)
                            _context.Entry(tripDay).State = EntityState.Modified;

                        Console.WriteLine($"[Edit] Saved new image: {tripDay.ImagePath}");
                    }
                    else
                    {
                        Console.WriteLine($"[Edit] No new image for day {i}");

                        // בדיקה אם ImagePath ריק - זה אומר שהמשתמש מחק את התמונה
                        if (string.IsNullOrEmpty(existingImagePath) && !string.IsNullOrEmpty(tripDay.ImagePath))
                        {
                            Console.WriteLine($"[Edit] Image was marked for deletion for day {i}");

                            // מחיקת הקובץ הפיזי
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath ?? "",
                                tripDay.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                            try
                            {
                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                    Console.WriteLine($"[Edit] Deleted image file: {oldImagePath}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Edit] Could not delete image: {ex.Message}");
                            }

                            tripDay.ImagePath = null;

                            if (tripDay.TripDayId > 0)
                                _context.Entry(tripDay).State = EntityState.Modified;
                        }
                    }
                }

                // מחיקת ימים שהוסרו
                var daysToRemove = existingTrip.TripDays
                    .Where(d => d.TripDayId > 0 && !existingDayIds.Contains(d.TripDayId))
                    .ToList();

                foreach (var dayToRemove in daysToRemove)
                {
                    Console.WriteLine($"[Edit] Removing day {dayToRemove.TripDayId} (DayNumber: {dayToRemove.DayNumber})");

                    // מחיקת תמונה פיזית אם קיימת
                    if (!string.IsNullOrEmpty(dayToRemove.ImagePath))
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath ?? "",
                            dayToRemove.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                        try
                        {
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                                Console.WriteLine($"[Edit] Deleted image: {imagePath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Edit] Could not delete image: {ex.Message}");
                        }
                    }

                    existingTrip.TripDays.Remove(dayToRemove);
                    _context.TripDays.Remove(dayToRemove);
                }

                // עדכון הישות הראשית
                _context.Entry(existingTrip).State = EntityState.Modified;

                Console.WriteLine("[Edit] Saving changes to database...");
                Console.WriteLine($"[Edit] Trip: {existingTrip.Title}");
                Console.WriteLine($"[Edit] Days count: {existingTrip.TripDays.Count}");

                foreach (var day in existingTrip.TripDays.OrderBy(d => d.DayNumber))
                {
                    var state = _context.Entry(day).State;
                    Console.WriteLine($"[Edit]   Day {day.DayNumber}: '{day.Title}' (ID: {day.TripDayId}, State: {state})");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("[Edit] Changes saved successfully!");

                TempData["SuccessMessage"] = $"הטיול '{existingTrip.Title}' עודכן בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"[Edit] Database error: {ex.Message}");
                Console.WriteLine($"[Edit] Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine(ex.StackTrace);

                ModelState.AddModelError("", $"שגיאה בעדכון הנתונים: {ex.InnerException?.Message ?? ex.Message}");

                // טען מחדש את הימים
                var vmTemp = await _context.Trips
                    .Include(t => t.TripDays)
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

                ModelState.AddModelError("", $"שגיאה: {ex.Message}");

                // טען מחדש את הימים
                var vmTemp = await _context.Trips
                    .Include(t => t.TripDays)
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
            var trip = await _context.Trips
                .Include(t => t.TripDays)
                .FirstOrDefaultAsync(t => t.TripId == id);

            if (trip != null)
            {
                // מחיקת תמונות
                foreach (var day in trip.TripDays)
                {
                    if (!string.IsNullOrEmpty(day.ImagePath))
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, day.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }

                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"הטיול '{trip.Title}' נמחק בהצלחה!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
    }
}