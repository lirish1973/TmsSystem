using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;

namespace TmsSystem.Controllers
{
    [Authorize]
    public class GuidesController : Controller
    {
        private readonly ApplicationDbContext _context;
        // הוסף קבוע בראש ה-Controller
        private const int PLACEHOLDER_GUIDE_ID = 9; 

        public GuidesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Guides
        public async Task<IActionResult> Index()
        {
            try
            {
                const int PLACEHOLDER_GUIDE_ID = 9; // 👈 המדריך הפיקטיבי

                // טען את כל המדריכים - חוץ מהמדריך הפיקטיבי
                var guides = await _context.Guides
                    .Where(g => g.GuideId != PLACEHOLDER_GUIDE_ID) // 👈 הסתרת המדריך הפיקטיבי
                    .OrderByDescending(g => g.IsActive)
                    .ThenBy(g => g.GuideName)
                    .ToListAsync();

                Console.WriteLine($"✅ Loaded {guides.Count} guides (excluding placeholder ID={PLACEHOLDER_GUIDE_ID})");
                Console.WriteLine($"   Active: {guides.Count(g => g.IsActive)}");
                Console.WriteLine($"   Inactive: {guides.Count(g => !g.IsActive)}");

                return View(guides);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading guides: {ex.Message}");
                TempData["ErrorMessage"] = $"שגיאה בטעינת המדריכים: {ex.Message}";
                return View(new List<Guide>());
            }
        }



        // POST: Guides/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, [FromBody] ToggleStatusRequest request)
        {
            try
            {
                var guide = await _context.Guides.FindAsync(id);

                if (guide == null)
                {
                    return Json(new { success = false, message = "המדריך לא נמצא" });
                }

                var oldStatus = guide.IsActive;
                guide.IsActive = request.IsActive;

                await _context.SaveChangesAsync();

                var statusText = request.IsActive ? "הופעל" : "הושבת";
                Console.WriteLine($"✅ Guide {id} ({guide.GuideName}) {statusText}");

                return Json(new
                {
                    success = true,
                    message = $"המדריך {statusText} בהצלחה"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error toggling guide status: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Guides/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guide = await _context.Guides
                .Include(g => g.Trips)
                .Include(g => g.Offers)
                .FirstOrDefaultAsync(m => m.GuideId == id);

            if (guide == null)
            {
                return NotFound();
            }

            return View(guide);
        }

        // GET: Guides/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guides/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GuideName,Description,Phone,Email")] Guide guide)
        {
            if (ModelState.IsValid)
            {
                guide.CreatedAt = DateTime.Now;
                guide.IsActive = true;

                _context.Add(guide);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"המדריך '{guide.GuideName}' נוסף בהצלחה!";
                return RedirectToAction(nameof(Index));
            }
            return View(guide);
        }

        // GET: Guides/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            const int PLACEHOLDER_GUIDE_ID = 9; // 👈 המדריך הפיקטיבי

            if (id == null)
            {
                return NotFound();
            }

            // מניעת עריכת המדריך הפיקטיבי
            if (id == PLACEHOLDER_GUIDE_ID)
            {
                TempData["ErrorMessage"] = "לא ניתן לערוך את המדריך הפיקטיבי";
                return RedirectToAction(nameof(Index));
            }

            var guide = await _context.Guides.FindAsync(id);

            if (guide == null)
            {
                return NotFound();
            }

            return View(guide);
        }




        // POST: Guides/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GuideId,GuideName,Description,Phone,Email,IsActive,CreatedAt")] Guide guide)
        {
            if (id != guide.GuideId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guide);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"המדריך '{guide.GuideName}' עודכן בהצלחה!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuideExists(guide.GuideId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(guide);
        }

        // GET: Guides/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guide = await _context.Guides
                .FirstOrDefaultAsync(m => m.GuideId == id);

            if (guide == null)
            {
                return NotFound();
            }

            return View(guide);
        }

        // POST: Guides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Console.WriteLine($"🗑️ Starting delete process for Guide ID: {id}");

                const int PLACEHOLDER_GUIDE_ID = 9; // 👈 המדריך הפיקטיבי

                // מניעת מחיקה של המדריך הפיקטיבי
                if (id == PLACEHOLDER_GUIDE_ID)
                {
                    TempData["ErrorMessage"] = "לא ניתן למחוק את המדריך הפיקטיבי! ";
                    return RedirectToAction(nameof(Index));
                }

                var guide = await _context.Guides.FindAsync(id);

                if (guide == null)
                {
                    Console.WriteLine($"❌ Guide {id} not found");
                    TempData["ErrorMessage"] = "המדריך לא נמצא";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"✅ Guide found: {guide.GuideName}");

                // וודא שהמדריך הפיקטיבי קיים
                var placeholderGuide = await _context.Guides.FindAsync(PLACEHOLDER_GUIDE_ID);
                if (placeholderGuide == null)
                {
                    Console.WriteLine("❌ Placeholder guide not found!  Creating.. .");

                    // זה לא אמור לקרות, אבל למקרה חירום
                    TempData["ErrorMessage"] = "שגיאה: המדריך הפיקטיבי לא נמצא במערכת";
                    return RedirectToAction(nameof(Index));
                }

                // העברת טיולים למדריך הפיקטיבי
                var linkedTrips = await _context.Trips
                    .Where(t => t.GuideId == id)
                    .ToListAsync();

                if (linkedTrips.Any())
                {
                    Console.WriteLine($"🔄 Moving {linkedTrips.Count} trips to placeholder guide (ID={PLACEHOLDER_GUIDE_ID})");

                    foreach (var trip in linkedTrips)
                    {
                        trip.GuideId = PLACEHOLDER_GUIDE_ID;
                    }

                    _context.UpdateRange(linkedTrips);
                }

                // העברת הצעות מחיר למדריך הפיקטיבי
                var linkedOffers = await _context.Offers
                    .Where(o => o.GuideId == id)
                    .ToListAsync();

                if (linkedOffers.Any())
                {
                    Console.WriteLine($"🔄 Moving {linkedOffers.Count} offers to placeholder guide (ID={PLACEHOLDER_GUIDE_ID})");

                    foreach (var offer in linkedOffers)
                    {
                        offer.GuideId = PLACEHOLDER_GUIDE_ID;
                    }

                    _context.UpdateRange(linkedOffers);
                }

                // מחיקת המדריך
                Console.WriteLine("🗑️ Removing guide from database");
                _context.Guides.Remove(guide);

                await _context.SaveChangesAsync();

                Console.WriteLine("✅ Guide deleted successfully!");
                TempData["SuccessMessage"] = $"המדריך '{guide.GuideName}' נמחק בהצלחה! ";

                if (linkedTrips.Any() || linkedOffers.Any())
                {
                    TempData["InfoMessage"] = $"הועברו {linkedTrips.Count} טיולים ו-{linkedOffers.Count} הצעות מחיר למדריך 'לא מוגדר'. ";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"❌ Database error: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                TempData["ErrorMessage"] = $"שגיאה במחיקת המדריך: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");

                TempData["ErrorMessage"] = $"שגיאה במחיקת המדריך: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }



        private bool GuideExists(int id)
        {
            return _context.Guides.Any(e => e.GuideId == id);
        }

        // DTO class for ToggleStatus
        public class ToggleStatusRequest
        {
            public bool IsActive { get; set; }
        }
    }
}