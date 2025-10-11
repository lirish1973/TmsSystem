﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.ViewModels;

namespace TmsSystem.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Tours/ - רשימת סיורים
        public async Task<IActionResult> Index()
        {
            var tours = await _context.Tours
                .Include(t => t.Schedule)   // ItineraryItems
                .Include(t => t.Includes)   // tourInclude
                .Include(t => t.Excludes)   // tourExclude
                .ToListAsync();

            return View(tours);
        }


        // GET: /Tours/CreateTour
        public IActionResult CreateTour()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateTourViewModel());
        }

        /// <summary>
        /// //
        /// ==========================================EDIT ==========================================
        ///
        ///
        ///
        ///




        [HttpPost]
        public async Task<IActionResult> EditTour(CreateTourViewModel model)
        {
            // הוסף debug כדי לראות מה מתקבל
            System.Diagnostics.Debug.WriteLine($"EditTour POST - TourId: {model.TourId}");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // הוסף debug נוסף
                System.Diagnostics.Debug.WriteLine($"Looking for tour with ID: {model.TourId}");

                // מצא את הסיור הקיים
                var existingTour = await _context.Tours
                    .Include(t => t.Schedule)
                    .Include(t => t.Includes)
                    .Include(t => t.Excludes)
                    .FirstOrDefaultAsync(t => t.TourId == model.TourId);

                if (existingTour == null)
                {
                    // הוסף debug למה הסיור לא נמצא
                    System.Diagnostics.Debug.WriteLine($"Tour with ID {model.TourId} not found in database");

                    // בדוק כמה סיורים יש בכלל
                    var totalTours = await _context.Tours.CountAsync();
                    System.Diagnostics.Debug.WriteLine($"Total tours in database: {totalTours}");

                    // הצג את כל ה-IDs הקיימים
                    var existingIds = await _context.Tours.Select(t => t.TourId).ToListAsync();
                    System.Diagnostics.Debug.WriteLine($"Existing tour IDs: {string.Join(", ", existingIds)}");

                    ModelState.AddModelError("", $"הסיור עם מזהה {model.TourId} לא נמצא במערכת");
                    return View(model);
                }

                // עדכן פרטי הסיור הבסיסיים
                existingTour.Title = model.Title;
                existingTour.Description = model.Description;

                // עדכן לוח זמנים (ItineraryItems)
                if (existingTour.Schedule != null && existingTour.Schedule.Any())
                {
                    _context.ItineraryItems.RemoveRange(existingTour.Schedule);
                }

                if (model.Schedule != null && model.Schedule.Any())
                {
                    var scheduleItems = model.Schedule.Select(s => new ItineraryItem
                    {
                        TourId = existingTour.TourId,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Location = s.Location,
                        Description = s.Description
                    }).ToList();

                    await _context.ItineraryItems.AddRangeAsync(scheduleItems);
                }

                // עדכן פריטים כלולים
                if (existingTour.Includes != null && existingTour.Includes.Any())
                {
                    _context.TourIncludes.RemoveRange(existingTour.Includes);
                }

                if (model.Includes != null && model.Includes.Any(i => !string.IsNullOrWhiteSpace(i)))
                {
                    var includeItems = model.Includes
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .Select(i => new TourInclude
                        {
                            TourId = existingTour.TourId,
                            Text = i,
                            Description = i
                        }).ToList();

                    await _context.TourIncludes.AddRangeAsync(includeItems);
                }

                // עדכן פריטים לא כלולים
                if (existingTour.Excludes != null && existingTour.Excludes.Any())
                {
                    _context.TourExcludes.RemoveRange(existingTour.Excludes);
                }

                if (model.Excludes != null && model.Excludes.Any(e => !string.IsNullOrWhiteSpace(e)))
                {
                    var excludeItems = model.Excludes
                        .Where(e => !string.IsNullOrWhiteSpace(e))
                        .Select(e => new TourExclude
                        {
                            TourId = existingTour.TourId,
                            Text = e,
                            Description = e
                        }).ToList();

                    await _context.TourExcludes.AddRangeAsync(excludeItems);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "הסיור עודכן בהצלחה";
                return RedirectToAction("Details", new { id = existingTour.TourId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EditTour: {ex.Message}");
                ModelState.AddModelError("", "אירעה שגיאה בעדכון הסיור. אנא נסה שוב.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditTour(int id)
        {
            System.Diagnostics.Debug.WriteLine($"EditTour GET - ID: {id}");

            var tour = await _context.Tours
                .Include(t => t.Schedule)
                .Include(t => t.Includes)
                .Include(t => t.Excludes)
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (tour == null)
            {
                System.Diagnostics.Debug.WriteLine($"Tour with ID {id} not found for editing");
                return NotFound("הסיור לא נמצא");
            }

            var model = new CreateTourViewModel
            {
                TourId = tour.TourId,
                Title = tour.Title ?? string.Empty,
                Description = tour.Description ?? string.Empty,
                Schedule = tour.Schedule?.Select(s => new ScheduleItemViewModel
                {
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Location = s.Location ?? string.Empty,
                    Description = s.Description ?? string.Empty
                }).ToList() ?? new List<ScheduleItemViewModel>(),
                Includes = tour.Includes?.Select(i => i.Text ?? string.Empty).ToList() ?? new List<string>(),
                Excludes = tour.Excludes?.Select(e => e.Text ?? string.Empty).ToList() ?? new List<string>()
            };

            System.Diagnostics.Debug.WriteLine($"Loaded tour for editing - ID: {model.TourId}, Title: {model.Title}");

            return View(model);
        }

























        // ==========================================EDIT ==========================================

        // GET: Tours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.Tours
                .Include(t => t.Schedule)
                .Include(t => t.Includes)
                .Include(t => t.Excludes)
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (tour == null) return NotFound();

            var model = new CreateTourViewModel
            {
                Title = tour.Title,
                Description = tour.Description,
                Schedule = tour.Schedule.Select(s => new ScheduleItemViewModel
                {
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Location = s.Location,
                    Description = s.Description
                }).ToList(),
                Includes = tour.Includes.Select(i => i.Description).ToList(),
                Excludes = tour.Excludes.Select(e => e.Description).ToList()
            };

            // שמירה של ה-TourId ב-TempData כדי להשתמש ב-POST
            TempData["EditingTourId"] = tour.TourId;

            return View(model);
        }

        // POST: Tours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateTourViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // משחזר את ה-TourId מ-TempData
            if (!TempData.ContainsKey("EditingTourId"))
                return NotFound();

            int tourId = (int)TempData["EditingTourId"];

            var tour = await _context.Tours
                .Include(t => t.Schedule)
                .Include(t => t.Includes)
                .Include(t => t.Excludes)
                .FirstOrDefaultAsync(t => t.TourId == tourId);

            if (tour == null) return NotFound();

            // עדכון פרטים כלליים
            tour.Title = model.Title;
            tour.Description = model.Description;

            // ---- עדכון לוח זמנים ----
            _context.ItineraryItems.RemoveRange(tour.Schedule);
            if (model.Schedule != null)
            {
                foreach (var item in model.Schedule.Where(i => !string.IsNullOrEmpty(i.Location)))
                {
                    _context.ItineraryItems.Add(new ItineraryItem
                    {
                        TourId = tour.TourId,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        Location = item.Location,
                        Description = item.Description
                    });
                }
            }

            // ---- עדכון Includes ----
            _context.TourIncludes.RemoveRange(tour.Includes);
            if (model.Includes != null)
            {
                foreach (var inc in model.Includes.Where(i => !string.IsNullOrWhiteSpace(i)))
                {
                    _context.TourIncludes.Add(new TourInclude
                    {
                        TourId = tour.TourId,
                        Description = inc
                    });
                }
            }

            // ---- עדכון Excludes ----
            _context.TourExcludes.RemoveRange(tour.Excludes);
            if (model.Excludes != null)
            {
                foreach (var exc in model.Excludes.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    _context.TourExcludes.Add(new TourExclude
                    {
                        TourId = tour.TourId,
                        Description = exc
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }






        //DELETE 

        // GET: Tours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.Tours
                .Include(t => t.Schedule)
                .Include(t => t.Includes)
                .Include(t => t.Excludes)
                .FirstOrDefaultAsync(m => m.TourId == id);

            if (tour == null) return NotFound();

            return View(tour);
        }

        // POST: Tours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Schedule)
                .Include(t => t.Includes)
                .Include(t => t.Excludes)
                .FirstOrDefaultAsync(m => m.TourId == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // POST: CreateTour
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour(CreateTourViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // יצירת Tour ושמירה ל־DB
            var tour = new Tour
            {
                Title = model.Title,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync(); // TourId מתמלא

            // יצירת Itinerary ושמירה
            var itinerary = new Itinerary
            {
                TourId = tour.TourId,
                Name = $"לוח זמנים - {tour.Title}"
            };
            _context.Itineraries.Add(itinerary);
            await _context.SaveChangesAsync(); // ItineraryId מתמלא


            Console.WriteLine($"Title: {model.Title}, Description: {model.Description}");

            // יצירת ItineraryItems
            if (model.Schedule != null)
            {
                foreach (var item in model.Schedule.Where(i => !string.IsNullOrEmpty(i.Location)))
                {
                    _context.ItineraryItems.Add(new ItineraryItem
                    {
                        TourId = tour.TourId,   // כאן המפתח הזר
                        ItineraryId = itinerary.ItineraryId,  // חובה!
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        Location = item.Location,
                        Description = item.Description
                    });

                }
            }

            // יצירת TourIncludes
            if (model.Includes != null)
            {
                foreach (var inc in model.Includes.Where(i => !string.IsNullOrWhiteSpace(i)))
                {
                    _context.TourIncludes.Add(new TourInclude
                    {
                        TourId = tour.TourId,
                        Description = inc
                    });
                }
            }

            // יצירת TourExcludes
            if (model.Excludes != null)
            {
                foreach (var exc in model.Excludes.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    _context.TourExcludes.Add(new TourExclude
                    {
                        TourId = tour.TourId,
                        Description = exc
                    });
                }
            }

            // שמירה סופית
            await _context.SaveChangesAsync();

            // החזרה למסך הרשימה
            return RedirectToAction(nameof(Index));
        }
    }
}
