using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;

namespace TmsSystem.Controllers
{
    [Authorize]
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hotels
        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .OrderBy(h => h.HotelName)
                .ToListAsync();
            return View(hotels);
        }

        // POST: Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string hotelName, string? location)
        {
            if (string.IsNullOrWhiteSpace(hotelName))
            {
                TempData["ErrorMessage"] = "שם בית המלון הוא שדה חובה.";
                return RedirectToAction(nameof(Index));
            }

            var hotel = new Hotel
            {
                HotelName = hotelName.Trim(),
                Location = location?.Trim(),
                IsActive = true
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"בית המלון '{hotel.HotelName}' נוסף בהצלחה!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Hotels/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            hotel.IsActive = !hotel.IsActive;
            await _context.SaveChangesAsync();

            var status = hotel.IsActive ? "פעיל" : "לא פעיל";
            TempData["SuccessMessage"] = $"'{hotel.HotelName}' עודכן ל-{status}";
            return RedirectToAction(nameof(Index));
        }

        // POST: Hotels/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            var name = hotel.HotelName;
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"בית המלון '{name}' נמחק.";
            return RedirectToAction(nameof(Index));
        }
    }
}
