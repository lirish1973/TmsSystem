using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsSystem.Data;
using TmsSystem.Models;
using TmsSystem.Services;

namespace TmsSystem.Controllers
{
    [Authorize]
    public class ImageGalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ImageGalleryController> _logger;
        private readonly IImageCompressionService _imageCompressionService;

        public ImageGalleryController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            ILogger<ImageGalleryController> logger,
            IImageCompressionService imageCompressionService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _imageCompressionService = imageCompressionService;
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

                // Compress and save image
                var compressionOptions = new ImageCompressionOptions
                {
                    MaxWidth = 1920,
                    MaxHeight = 1080,
                    JpegQuality = 75,
                    MaxFileSizeKB = 200
                };

                var compressionResult = await _imageCompressionService.CompressAndSaveAsync(
                    file, filePath, compressionOptions);

                if (!compressionResult.Success)
                {
                    _logger.LogError("Failed to compress image: {Error}", compressionResult.ErrorMessage);
                    TempData["ErrorMessage"] = $"שגיאה בדחיסת תמונה: {compressionResult.ErrorMessage}";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation(
                    "Image compressed: Original {OrigSize}KB -> Compressed {CompSize}KB (Saved {Ratio}%)",
                    compressionResult.OriginalSizeBytes / 1024,
                    compressionResult.CompressedSizeBytes / 1024,
                    compressionResult.CompressionRatio);

                // Create database record
                var imageGallery = new ImageGallery
                {
                    FileName = file.FileName,
                    FilePath = $"/uploads/gallery/{fileName}",
                    Description = description,
                    FileSize = compressionResult.CompressedSizeBytes, // Save compressed size
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
