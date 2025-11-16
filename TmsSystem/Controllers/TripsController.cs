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
        public async Task<IActionResult> Create(Trip trip, List<IFormFile>? dayImages)
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

                    // טיפול בתמונה של אותו יום
                    if (dayImages != null && i < dayImages.Count && dayImages[i] != null && dayImages[i].Length > 0)
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dayImages[i].FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await dayImages[i].CopyToAsync(fileStream);
                        }

                        tripDay.ImagePath = "/uploads/trips/" + uniqueFileName;
                    }

                    tripDaysList.Add(tripDay);
                }

                // יצירת הטיול
                var newTrip = new Trip
                {
                    Title = trip.Title,
                    Description = trip.Description,
                    NumberOfDays = trip.NumberOfDays,
                    IsActive = trip.IsActive,
                    CreatedAt = DateTime.Now,
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
        public async Task<IActionResult> Edit(int id, Trip trip, List<IFormFile>? dayImages)
        {
            if (id != trip.TripId)
            {
                return NotFound();
            }

            try
            {
                // מציאת הטיול הקיים
                var existingTrip = await _context.Trips
                    .Include(t => t.TripDays)
                    .FirstOrDefaultAsync(t => t.TripId == id);

                if (existingTrip == null)
                {
                    return NotFound();
                }

                // עדכון פרטי הטיול הבסיסיים
                existingTrip.Title = trip.Title;
                existingTrip.Description = trip.Description;
                existingTrip.IsActive = trip.IsActive;
                existingTrip.NumberOfDays = trip.NumberOfDays;

                // יצירת תיקיית uploads אם לא קיימת
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "trips");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // אוסף של ימים חדשים/מעודכנים
                var updatedDays = new List<TripDay>();
                var existingDayIds = new HashSet<int>();

                // עיבוד כל הימים מהטופס
                for (int i = 0; i < trip.NumberOfDays; i++)
                {
                    var tripDayIdStr = Request.Form[$"TripDays[{i}].TripDayId"].ToString();
                    var tripDayId = int.Parse(string.IsNullOrEmpty(tripDayIdStr) ? "0" : tripDayIdStr);
                    var dayNumber = int.Parse(Request.Form[$"TripDays[{i}].DayNumber"].ToString());
                    var title = Request.Form[$"TripDays[{i}].Title"].ToString();
                    var location = Request.Form[$"TripDays[{i}].Location"].ToString();
                    var description = Request.Form[$"TripDays[{i}].Description"].ToString();
                    var existingImagePath = Request.Form[$"TripDays[{i}].ImagePath"].ToString();

                    TripDay tripDay;

                    if (tripDayId > 0)
                    {
                        // יום קיים - עדכון
                        tripDay = existingTrip.TripDays.FirstOrDefault(d => d.TripDayId == tripDayId);
                        if (tripDay != null)
                        {
                            tripDay.Title = title;
                            tripDay.Location = location;
                            tripDay.Description = description;
                            tripDay.DayNumber = dayNumber;
                            tripDay.DisplayOrder = dayNumber;
                            existingDayIds.Add(tripDayId);
                        }
                        else
                        {
                            continue; // יום לא נמצא, דלג
                        }
                    }
                    else
                    {
                        // יום חדש - יצירה
                        tripDay = new TripDay
                        {
                            TripId = existingTrip.TripId,
                            DayNumber = dayNumber,
                            Title = title,
                            Location = location,
                            Description = description,
                            DisplayOrder = dayNumber,
                            ImagePath = existingImagePath
                        };
                        existingTrip.TripDays.Add(tripDay);
                    }

                    // טיפול בתמונה
                    if (dayImages != null && i < dayImages.Count && dayImages[i] != null && dayImages[i].Length > 0)
                    {
                        // מחיקת תמונה ישנה אם קיימת
                        if (!string.IsNullOrEmpty(tripDay.ImagePath))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, tripDay.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // שמירת תמונה חדשה
                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dayImages[i].FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await dayImages[i].CopyToAsync(fileStream);
                        }

                        tripDay.ImagePath = "/uploads/trips/" + uniqueFileName;
                    }
                    else if (string.IsNullOrEmpty(existingImagePath))
                    {
                        // אם אין תמונה חדשה ואין תמונה קיימת
                        tripDay.ImagePath = null;
                    }

                    updatedDays.Add(tripDay);
                }

                // מחיקת ימים שנמחקו (ימים שהיו קיימים אבל לא בטופס)
                var daysToRemove = existingTrip.TripDays
                    .Where(d => d.TripDayId > 0 && !existingDayIds.Contains(d.TripDayId))
                    .ToList();

                foreach (var dayToRemove in daysToRemove)
                {
                    // מחיקת תמונה אם קיימת
                    if (!string.IsNullOrEmpty(dayToRemove.ImagePath))
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, dayToRemove.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    existingTrip.TripDays.Remove(dayToRemove);
                    _context.TripDays.Remove(dayToRemove);
                }

                _context.Update(existingTrip);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"הטיול '{existingTrip.Title}' עודכן בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(trip.TripId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"שגיאה בעדכון הטיול: {ex.Message}");
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