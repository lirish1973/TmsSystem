using Microsoft.AspNetCore.Mvc;
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
