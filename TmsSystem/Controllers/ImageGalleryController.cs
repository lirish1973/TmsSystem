using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;

namespace TmsSystem.Controllers
{
    [Authorize]
    public class ImageGalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ImageGalleryController> _logger;

        public ImageGalleryController(
            ApplicationDbContext context, 
            IWebHostEnvironment webHostEnvironment,
            ILogger<ImageGalleryController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // GET: ImageGallery
        public async Task<IActionResult> Index()
        {
            var images = await _context.ImageGalleries
                .Where(i => i.IsActive)
                .OrderByDescending(i => i.UploadedAt)
                .ToListAsync();

            return View(images);
        }

        // POST: ImageGallery/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string? description)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["ErrorMessage"] = "לא נבחר קובץ להעלאה";
                    return RedirectToAction(nameof(Index));
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = "ניתן להעלות רק קבצי תמונה (JPG, PNG, GIF, WEBP)";
                    return RedirectToAction(nameof(Index));
                }

                // Create uploads folder if it doesn't exist
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "gallery");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create database record
                var imageGallery = new ImageGallery
                {
                    FileName = file.FileName,
                    FilePath = $"/uploads/gallery/{fileName}",
                    Description = description,
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0
                };

                _context.ImageGalleries.Add(imageGallery);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Image uploaded successfully: {FileName}", fileName);
                TempData["SuccessMessage"] = "התמונה הועלתה בהצלחה לספרייה";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                TempData["ErrorMessage"] = $"שגיאה בהעלאת תמונה: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ImageGallery/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var image = await _context.ImageGalleries.FindAsync(id);
                
                if (image == null)
                {
                    TempData["ErrorMessage"] = "התמונה לא נמצאה";
                    return RedirectToAction(nameof(Index));
                }

                // Check if image is in use
                var tripsUsingImage = await _context.TripDays
                    .Where(td => td.ImagePath == image.FilePath)
                    .ToListAsync();

                if (tripsUsingImage.Any())
                {
                    // Replace with placeholder in all trips using this image
                    foreach (var tripDay in tripsUsingImage)
                    {
                        tripDay.ImagePath = "/images/placeholder-image.svg";
                    }

                    _logger.LogWarning("Image {ImageId} was used in {Count} trip days, replaced with placeholder", 
                        id, tripsUsingImage.Count);
                    TempData["InfoMessage"] = $"התמונה הייתה בשימוש ב-{tripsUsingImage.Count} ימי טיול והוחלפה בתמונת ברירת מחדל";
                }

                // Delete physical file
                var physicalPath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    image.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                    _logger.LogInformation("Deleted physical file: {PhysicalPath}", physicalPath);
                }

                // Mark as inactive (soft delete)
                image.IsActive = false;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "התמונה נמחקה בהצלחה";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId}", id);
                TempData["ErrorMessage"] = $"שגיאה במחיקת תמונה: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ImageGallery/GetImages (for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var images = await _context.ImageGalleries
                .Where(i => i.IsActive)
                .OrderByDescending(i => i.UploadedAt)
                .Select(i => new
                {
                    i.ImageId,
                    i.FileName,
                    i.FilePath,
                    i.Description,
                    i.UsageCount
                })
                .ToListAsync();

            return Json(images);
        }
    }
}
