using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;

namespace TmsSystem.Services
{
    public interface IImageCompressionService
    {
        Task<ImageCompressionResult> CompressAndSaveAsync(IFormFile file, string destinationPath, ImageCompressionOptions? options = null);
        Task<ImageCompressionResult> CompressAndSaveAsync(Stream sourceStream, string destinationPath, string originalFileName, ImageCompressionOptions? options = null);
    }

    public class ImageCompressionOptions
    {
        /// <summary>
        /// רוחב מקסימלי בפיקסלים (ברירת מחדל: 1920)
        /// </summary>
        public int MaxWidth { get; set; } = 1920;

        /// <summary>
        /// גובה מקסימלי בפיקסלים (ברירת מחדל: 1080)
        /// </summary>
        public int MaxHeight { get; set; } = 1080;

        /// <summary>
        /// איכות JPEG (1-100, ברירת מחדל: 75)
        /// </summary>
        public int JpegQuality { get; set; } = 75;

        /// <summary>
        /// רמת דחיסה ל-PNG (0-9, ברירת מחדל: 6)
        /// </summary>
        public int PngCompressionLevel { get; set; } = 6;

        /// <summary>
        /// איכות WebP (1-100, ברירת מחדל: 75)
        /// </summary>
        public int WebPQuality { get; set; } = 75;

        /// <summary>
        /// האם להמיר ל-JPEG (מומלץ לתמונות עם הרבה צבעים)
        /// </summary>
        public bool ConvertToJpeg { get; set; } = false;

        /// <summary>
        /// גודל קובץ מקסימלי ב-KB לפני שנדרשת דחיסה (ברירת מחדל: 200KB)
        /// </summary>
        public long MaxFileSizeKB { get; set; } = 200;
    }

    public class ImageCompressionResult
    {
        public bool Success { get; set; }
        public string? FilePath { get; set; }
        public string? ErrorMessage { get; set; }
        public long OriginalSizeBytes { get; set; }
        public long CompressedSizeBytes { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public int FinalWidth { get; set; }
        public int FinalHeight { get; set; }
        public double CompressionRatio => OriginalSizeBytes > 0
            ? Math.Round((1 - (double)CompressedSizeBytes / OriginalSizeBytes) * 100, 1)
            : 0;
    }

    public class ImageCompressionService : IImageCompressionService
    {
        private readonly ILogger<ImageCompressionService> _logger;

        public ImageCompressionService(ILogger<ImageCompressionService> logger)
        {
            _logger = logger;
        }

        public async Task<ImageCompressionResult> CompressAndSaveAsync(
            IFormFile file,
            string destinationPath,
            ImageCompressionOptions? options = null)
        {
            using var stream = file.OpenReadStream();
            return await CompressAndSaveAsync(stream, destinationPath, file.FileName, options);
        }

        public async Task<ImageCompressionResult> CompressAndSaveAsync(
            Stream sourceStream,
            string destinationPath,
            string originalFileName,
            ImageCompressionOptions? options = null)
        {
            options ??= new ImageCompressionOptions();
            var result = new ImageCompressionResult();

            try
            {
                // קריאת הגודל המקורי
                result.OriginalSizeBytes = sourceStream.Length;

                // טעינת התמונה
                using var image = await Image.LoadAsync(sourceStream);

                result.OriginalWidth = image.Width;
                result.OriginalHeight = image.Height;

                _logger.LogInformation(
                    "Processing image: {FileName}, Original size: {Size}KB, Dimensions: {Width}x{Height}",
                    originalFileName,
                    result.OriginalSizeBytes / 1024,
                    image.Width,
                    image.Height);

                // בדיקה אם צריך לשנות גודל
                bool needsResize = image.Width > options.MaxWidth || image.Height > options.MaxHeight;
                bool needsCompression = result.OriginalSizeBytes > options.MaxFileSizeKB * 1024;

                if (needsResize)
                {
                    // שינוי גודל תוך שמירה על יחס הגובה-רוחב
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(options.MaxWidth, options.MaxHeight),
                        Mode = ResizeMode.Max // שומר על יחס הגובה-רוחב
                    }));

                    _logger.LogInformation(
                        "Resized image from {OrigW}x{OrigH} to {NewW}x{NewH}",
                        result.OriginalWidth, result.OriginalHeight,
                        image.Width, image.Height);
                }

                result.FinalWidth = image.Width;
                result.FinalHeight = image.Height;

                // קביעת פורמט השמירה
                var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

                // שמירה עם דחיסה
                await using var outputStream = new FileStream(destinationPath, FileMode.Create);

                if (options.ConvertToJpeg || extension == ".jpg" || extension == ".jpeg")
                {
                    // שמירה כ-JPEG עם דחיסה
                    var encoder = new JpegEncoder
                    {
                        Quality = options.JpegQuality
                    };
                    await image.SaveAsJpegAsync(outputStream, encoder);
                }
                else if (extension == ".png")
                {
                    // שמירה כ-PNG עם דחיסה
                    var encoder = new PngEncoder
                    {
                        CompressionLevel = (PngCompressionLevel)Math.Min(options.PngCompressionLevel, 9)
                    };
                    await image.SaveAsPngAsync(outputStream, encoder);
                }
                else if (extension == ".webp")
                {
                    // שמירה כ-WebP עם דחיסה
                    var encoder = new WebpEncoder
                    {
                        Quality = options.WebPQuality
                    };
                    await image.SaveAsWebpAsync(outputStream, encoder);
                }
                else if (extension == ".gif")
                {
                    // GIF - שמירה רגילה (ImageSharp תומך בדחיסת GIF בסיסית)
                    await image.SaveAsGifAsync(outputStream);
                }
                else
                {
                    // ברירת מחדל - JPEG
                    var encoder = new JpegEncoder
                    {
                        Quality = options.JpegQuality
                    };
                    await image.SaveAsJpegAsync(outputStream, encoder);
                }

                result.CompressedSizeBytes = outputStream.Length;
                result.FilePath = destinationPath;
                result.Success = true;

                _logger.LogInformation(
                    "Image compressed successfully: {FileName}, " +
                    "Original: {OrigSize}KB, Compressed: {CompSize}KB, " +
                    "Reduction: {Ratio}%",
                    originalFileName,
                    result.OriginalSizeBytes / 1024,
                    result.CompressedSizeBytes / 1024,
                    result.CompressionRatio);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compressing image: {FileName}", originalFileName);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }
    }
}
