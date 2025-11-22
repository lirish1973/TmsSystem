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

        public GuidesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Guides
        public async Task<IActionResult> Index()
        {
            var guides = await _context.Guides
                .Where(g => g.IsActive)
                .OrderBy(g => g.GuideName)
                .ToListAsync();

            return View(guides);
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
        public async Task<IActionResult> Create([Bind("GuideName,Description,Phone,Email,LicenseNumber")] Guide guide)
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
            if (id == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Edit(int id, [Bind("GuideId,GuideName,Description,Phone,Email,LicenseNumber,IsActive")] Guide guide)
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
                return RedirectToAction(nameof(Index));
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
            var guide = await _context.Guides.FindAsync(id);
            if (guide != null)
            {
                // Soft delete - לא מוחקים ממש, רק מסמנים כלא פעיל
                guide.IsActive = false;
                _context.Update(guide);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"המדריך '{guide.GuideName}' הוסר בהצלחה!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GuideExists(int id)
        {
            return _context.Guides.Any(e => e.GuideId == id);
        }
    }
}