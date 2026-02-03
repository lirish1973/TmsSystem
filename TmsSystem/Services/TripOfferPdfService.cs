using System.Text;
using System.Net;
using System.Web;
using TmsSystem.Models;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace TmsSystem.Services
{
    public class TripOfferPdfService : ITripOfferPdfService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TripOfferPdfService> _logger;

        public TripOfferPdfService(IWebHostEnvironment env, ILogger<TripOfferPdfService> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// HTML-encodes text for safe use in PDF generation
        /// </summary>
        private string EncodeHtml(string? text, string defaultValue = "לא צוין")
        {
            var textToUse = text ?? defaultValue;
            return HttpUtility.HtmlEncode(textToUse);
        }

        public async Task<byte[]> GenerateTripOfferPdfAsync(TripOffer offer)
        {
            var html = await GenerateTripOfferHtmlAsync(offer);

            try
            {
                // Download browser if not exists
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();

                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
                });

                await using var page = await browser.NewPageAsync();
                await page.SetContentAsync(html);

                var pdfBytes = await page.PdfDataAsync(new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true,
                    MarginOptions = new MarginOptions
                    {
                        Top = "15mm",
                        Bottom = "15mm",
                        Left = "10mm",
                        Right = "10mm"
                    }
                });

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate trip offer PDF for offer: {OfferId}", offer?.OfferNumber);
                throw new InvalidOperationException($"Failed to generate PDF: {ex.Message}", ex);
            }
        }

        public async Task<string> GenerateTripOfferHtmlAsync(TripOffer offer)
        {
            var html = new StringBuilder();

            // Get ordered trip days
            var orderedDays = offer.Trip?.TripDays?.OrderBy(d => d.DayNumber).ToList() ?? new List<TripDay>();

            // Professional HTML with proper RTL support
            html.AppendLine(@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <title>הצעת מחיר לטיול - TRYIT</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Heebo:wght@300;400;500;600;700&display=swap');

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Heebo', 'Segoe UI', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #fff;
            direction: rtl;
            font-size: 14px;
        }

        .container {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }

        /* Header */
        .header {
            background: #1a365d;
            color: white;
            padding: 30px;
            text-align: center;
            margin-bottom: 30px;
        }

        .header .logo {
            font-size: 28px;
            font-weight: 700;
            letter-spacing: 2px;
            margin-bottom: 10px;
        }

        .header h1 {
            font-size: 22px;
            font-weight: 500;
            margin: 10px 0;
        }

        .header .offer-info {
            font-size: 13px;
            opacity: 0.9;
            margin-top: 10px;
        }

        /* Section styling */
        .section {
            margin-bottom: 25px;
            page-break-inside: avoid;
        }

        .section-title {
            font-size: 16px;
            font-weight: 600;
            color: #1a365d;
            padding: 10px 15px;
            background: #f7fafc;
            border-right: 4px solid #1a365d;
            margin-bottom: 15px;
        }

        .section-content {
            padding: 0 15px;
        }

        /* Info rows */
        .info-table {
            width: 100%;
            border-collapse: collapse;
        }

        .info-table td {
            padding: 10px 0;
            border-bottom: 1px solid #e2e8f0;
            vertical-align: top;
        }

        .info-table tr:last-child td {
            border-bottom: none;
        }

        .info-label {
            font-weight: 600;
            color: #4a5568;
            width: 140px;
        }

        .info-value {
            color: #2d3748;
        }

        /* Trip days */
        .trip-day {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 6px;
            margin-bottom: 15px;
            overflow: hidden;
            page-break-inside: avoid;
        }

        .trip-day-header {
            background: #1a365d;
            color: white;
            padding: 10px 15px;
            font-weight: 600;
        }

        .trip-day-content {
            padding: 15px;
            display: flex;
            gap: 15px;
        }

        .trip-day-image {
            flex-shrink: 0;
            width: 200px;
            height: 140px;
            border-radius: 4px;
            overflow: hidden;
        }

        .trip-day-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .trip-day-text {
            flex: 1;
        }

        .trip-day-title {
            font-size: 15px;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 5px;
        }

        .trip-day-location {
            color: #1a365d;
            font-weight: 500;
            margin-bottom: 8px;
            font-size: 13px;
        }

        .trip-day-description {
            color: #4a5568;
            font-size: 13px;
            line-height: 1.6;
        }

        /* Lists */
        .check-list {
            list-style: none;
            padding: 0;
        }

        .check-list li {
            padding: 8px 0;
            padding-right: 25px;
            position: relative;
            border-bottom: 1px solid #f0f0f0;
        }

        .check-list li:last-child {
            border-bottom: none;
        }

        .check-list.includes li::before {
            content: '✓';
            position: absolute;
            right: 0;
            color: #38a169;
            font-weight: bold;
        }

        .check-list.excludes li::before {
            content: '✗';
            position: absolute;
            right: 0;
            color: #e53e3e;
            font-weight: bold;
        }

        /* Price section */
        .price-section {
            background: #f0fff4;
            border: 2px solid #38a169;
            border-radius: 6px;
            padding: 20px;
        }

        .price-section .section-title {
            background: #38a169;
            color: white;
            border-right: none;
            margin: -20px -20px 15px -20px;
            padding: 12px 20px;
        }

        .price-total {
            font-size: 24px;
            font-weight: 700;
            color: #276749;
        }

        /* Terms section */
        .terms-section {
            background: #fffaf0;
            border: 1px solid #ed8936;
            border-radius: 6px;
            padding: 20px;
        }

        .terms-section .section-title {
            background: #ed8936;
            color: white;
            border-right: none;
            margin: -20px -20px 15px -20px;
            padding: 12px 20px;
        }

        .term-item {
            padding: 10px;
            background: white;
            border-radius: 4px;
            margin-bottom: 10px;
            border-right: 3px solid #ed8936;
        }

        .term-item:last-child {
            margin-bottom: 0;
        }

        .term-item strong {
            color: #c05621;
        }

        /* Bank details */
        .bank-section {
            background: #ebf8ff;
            border: 1px solid #4299e1;
            border-radius: 6px;
            padding: 15px;
            margin-top: 15px;
        }

        .bank-section .section-title {
            background: #4299e1;
            color: white;
            border-right: none;
            margin: -15px -15px 10px -15px;
            padding: 10px 15px;
            font-size: 14px;
        }

        .bank-section .info-table td {
            padding: 6px 0;
        }

        /* Contact section */
        .contact-section {
            background: #1a365d;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 6px;
        }

        .contact-section h3 {
            margin-bottom: 15px;
        }

        .contact-info {
            font-size: 15px;
        }

        /* Footer */
        .footer {
            text-align: center;
            padding: 20px;
            margin-top: 20px;
            background: #fef3c7;
            border: 1px solid #f6ad55;
            border-radius: 6px;
        }

        .footer strong {
            color: #c05621;
        }
    </style>
</head>
<body>
    <div class='container'>");

            // Header
            html.AppendLine($@"
        <div class='header'>
            <div class='logo'>TRYIT</div>
            <h1>הצעת מחיר לטיול</h1>
            <div class='offer-info'>
                מספר הצעה: {EncodeHtml(offer.OfferNumber, "")} | תאריך: {offer.OfferDate:dd/MM/yyyy}
            </div>
        </div>");

            // Customer details
            html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>פרטי הלקוח</div>
            <div class='section-content'>
                <table class='info-table'>
                    <tr>
                        <td class='info-label'>שם מלא:</td>
                        <td class='info-value'>{EncodeHtml(offer.Customer?.DisplayName)}</td>
                    </tr>
                    <tr>
                        <td class='info-label'>טלפון:</td>
                        <td class='info-value'>{EncodeHtml(offer.Customer?.Phone)}</td>
                    </tr>
                    <tr>
                        <td class='info-label'>אימייל:</td>
                        <td class='info-value'>{EncodeHtml(offer.Customer?.Email)}</td>
                    </tr>");

            if (!string.IsNullOrEmpty(offer.Customer?.Address))
            {
                html.AppendLine($@"
                    <tr>
                        <td class='info-label'>כתובת:</td>
                        <td class='info-value'>{EncodeHtml(offer.Customer.Address)}</td>
                    </tr>");
            }

            html.AppendLine(@"
                </table>
            </div>
        </div>");

            // Trip details
            html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>פרטי הטיול</div>
            <div class='section-content'>
                <table class='info-table'>
                    <tr>
                        <td class='info-label'>שם הטיול:</td>
                        <td class='info-value'>{EncodeHtml(offer.Trip?.Title)}</td>
                    </tr>
                    <tr>
                        <td class='info-label'>משך הטיול:</td>
                        <td class='info-value'>{offer.Trip?.NumberOfDays ?? 0} ימים</td>
                    </tr>
                    <tr>
                        <td class='info-label'>תאריך יציאה:</td>
                        <td class='info-value'>{offer.DepartureDate:dd/MM/yyyy}</td>
                    </tr>");

            if (offer.ReturnDate.HasValue)
            {
                html.AppendLine($@"
                    <tr>
                        <td class='info-label'>תאריך חזרה:</td>
                        <td class='info-value'>{offer.ReturnDate.Value:dd/MM/yyyy}</td>
                    </tr>");
            }

            html.AppendLine($@"
                    <tr>
                        <td class='info-label'>מספר משתתפים:</td>
                        <td class='info-value'>{offer.Participants}</td>
                    </tr>
                </table>
            </div>
        </div>");

            // Trip description
            if (!string.IsNullOrEmpty(offer.Trip?.Description))
            {
                html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>אודות הטיול</div>
            <div class='section-content'>
                <p>{EncodeHtml(offer.Trip.Description).Replace("\n", "<br/>")}</p>
            </div>
        </div>");
            }

            // Trip days
            if (orderedDays.Any())
            {
                html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>תוכנית הטיול</div>
            <div class='section-content'>");

                foreach (var day in orderedDays)
                {
                    html.AppendLine($@"
                <div class='trip-day'>
                    <div class='trip-day-header'>יום {day.DayNumber}</div>
                    <div class='trip-day-content'>");

                    // Image
                    if (!string.IsNullOrWhiteSpace(day.ImagePath))
                    {
                        var imageTag = await GetImageTag(day.ImagePath);
                        if (!string.IsNullOrEmpty(imageTag))
                        {
                            html.AppendLine($@"
                        <div class='trip-day-image'>{imageTag}</div>");
                        }
                    }

                    html.AppendLine($@"
                        <div class='trip-day-text'>
                            <div class='trip-day-title'>{EncodeHtml(day.Title, "")}</div>");

                    if (!string.IsNullOrWhiteSpace(day.Location))
                    {
                        html.AppendLine($@"
                            <div class='trip-day-location'>{EncodeHtml(day.Location)}</div>");
                    }

                    if (!string.IsNullOrWhiteSpace(day.Description))
                    {
                        html.AppendLine($@"
                            <div class='trip-day-description'>{EncodeHtml(day.Description).Replace("\n", "<br/>")}</div>");
                    }

                    html.AppendLine(@"
                        </div>
                    </div>
                </div>");
                }

                html.AppendLine(@"
            </div>
        </div>");
            }

            // What's included
            if (!string.IsNullOrEmpty(offer.Trip?.Includes))
            {
                var includesList = offer.Trip.Includes.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (includesList.Any())
                {
                    html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>מה כלול במחיר</div>
            <div class='section-content'>
                <ul class='check-list includes'>");

                    foreach (var item in includesList)
                    {
                        html.AppendLine($@"
                    <li>{EncodeHtml(item.Trim())}</li>");
                    }

                    html.AppendLine(@"
                </ul>
            </div>
        </div>");
                }
            }

            // What's not included
            if (!string.IsNullOrEmpty(offer.Trip?.Excludes))
            {
                var excludesList = offer.Trip.Excludes.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (excludesList.Any())
                {
                    html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>מה לא כלול במחיר</div>
            <div class='section-content'>
                <ul class='check-list excludes'>");

                    foreach (var item in excludesList)
                    {
                        html.AppendLine($@"
                    <li>{EncodeHtml(item.Trim())}</li>");
                    }

                    html.AppendLine(@"
                </ul>
            </div>
        </div>");
                }
            }

            // Flight details
            if (offer.FlightIncluded && !string.IsNullOrEmpty(offer.FlightDetails))
            {
                html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>פרטי טיסה</div>
            <div class='section-content'>
                <p>{EncodeHtml(offer.FlightDetails).Replace("\n", "<br/>")}</p>
            </div>
        </div>");
            }

            // Special requests
            if (!string.IsNullOrEmpty(offer.SpecialRequests))
            {
                html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>בקשות מיוחדות</div>
            <div class='section-content'>
                <p>{EncodeHtml(offer.SpecialRequests).Replace("\n", "<br/>")}</p>
            </div>
        </div>");
            }

            // Terms
            html.AppendLine(@"
        <div class='section terms-section'>
            <div class='section-title'>תנאי ביטול ותשלום</div>
            <div class='term-item'>
                <strong>תשלום מקדמה:</strong> על מנת לשריין את הטיול יש להעביר מקדמה של 50% עם אישור ההזמנה. יש לסיים את כל התשלום עד 7 ימים עסקים מיום יציאת הטיול.
            </div>
            <div class='term-item'>
                <strong>דחיית תאריך:</strong> במידה ותרצו לדחות את התאריך עד 14 יום לפני הסיור ניתן לעשות זאת ללא עלות.
            </div>
            <div class='term-item'>
                <strong>ביטול עד 30 יום:</strong> במידה ותרצו לבטל את הסיור עד 30 יום לפני הסיור, תקבלו החזר פחות 300 ש״ח דמי טיפול.
            </div>
            <div class='term-item'>
                <strong>ביטול 30-14 ימים:</strong> ביטול שיתקיים בין 30 יום ל-14 יום לפני מועד הסיור - יגבו דמי ביטול של 50% מהמחיר.
            </div>
            <div class='term-item'>
                <strong>ביטול עד 14 ימים:</strong> ביטול שיתקיים בין 14 יום ליום הסיור - יגבו דמי ביטול מלאים.
            </div>
            <div class='term-item'>
                <strong>ביטול מטעמנו:</strong> במידה ולא ניתן לקיים את הסיור, ואנו נבטל אותו בשל תנאים ביטחוניים או תנאי מזג אויר, ולא תרצו מועד חלופי - תשלום מלא יוחזר.
            </div>
        </div>");

            // Price breakdown
            html.AppendLine($@"
        <div class='section price-section'>
            <div class='section-title'>פירוט מחירים</div>
            <table class='info-table'>
                <tr>
                    <td class='info-label'>מחיר לאדם:</td>
                    <td class='info-value'><strong>₪{offer.PricePerPerson:N2}</strong></td>
                </tr>
                <tr>
                    <td class='info-label'>מספר משתתפים:</td>
                    <td class='info-value'>× {offer.Participants}</td>
                </tr>
                <tr>
                    <td class='info-label'>סכום ביניים:</td>
                    <td class='info-value'>₪{(offer.PricePerPerson * offer.Participants):N2}</td>
                </tr>");

            if (offer.SingleRooms > 0 && offer.SingleRoomSupplement.HasValue)
            {
                html.AppendLine($@"
                <tr>
                    <td class='info-label'>תוספת חדרים יחידים:</td>
                    <td class='info-value'>{offer.SingleRooms} × ₪{offer.SingleRoomSupplement.Value:N2} = ₪{(offer.SingleRoomSupplement.Value * offer.SingleRooms):N2}</td>
                </tr>");
            }

            if (offer.InsuranceIncluded && offer.InsurancePrice.HasValue)
            {
                html.AppendLine($@"
                <tr>
                    <td class='info-label'>ביטוח נסיעות:</td>
                    <td class='info-value'>{offer.Participants} × ₪{offer.InsurancePrice.Value:N2} = ₪{(offer.InsurancePrice.Value * offer.Participants):N2}</td>
                </tr>");
            }

            html.AppendLine($@"
                <tr>
                    <td class='info-label'><strong>סה""כ לתשלום:</strong></td>
                    <td class='info-value'><span class='price-total'>₪{offer.TotalPrice:N2}</span></td>
                </tr>");

            if (offer.PaymentMethod != null)
            {
                html.AppendLine($@"
                <tr>
                    <td class='info-label'>אמצעי תשלום:</td>
                    <td class='info-value'>{EncodeHtml(offer.PaymentMethod.PaymentName)}</td>
                </tr>");

                if (offer.PaymentInstallments.HasValue)
                {
                    html.AppendLine($@"
                <tr>
                    <td class='info-label'>מספר תשלומים:</td>
                    <td class='info-value'>{offer.PaymentInstallments.Value} תשלומים</td>
                </tr>");
                }
            }

            html.AppendLine(@"
            </table>");

            // Bank details
            html.AppendLine($@"
            <div class='bank-section'>
                <div class='section-title'>פרטי העברה בנקאית</div>
                <table class='info-table'>
                    <tr>
                        <td class='info-label'>שם הבנק:</td>
                        <td class='info-value'>בנק לאומי</td>
                    </tr>
                    <tr>
                        <td class='info-label'>מספר סניף:</td>
                        <td class='info-value'>805</td>
                    </tr>
                    <tr>
                        <td class='info-label'>מספר חשבון:</td>
                        <td class='info-value'>39820047</td>
                    </tr>
                    <tr>
                        <td class='info-label'>שם בעל החשבון:</td>
                        <td class='info-value'>ספארי אפריקה בע״מ</td>
                    </tr>
                    <tr>
                        <td class='info-label'>ח.פ:</td>
                        <td class='info-value'>515323970</td>
                    </tr>
                    <tr>
                        <td class='info-label'><strong>סכום להעברה:</strong></td>
                        <td class='info-value'><strong style='color: #38a169;'>₪{offer.TotalPrice:N2}</strong></td>
                    </tr>
                </table>
            </div>
        </div>");

            // Contact
            html.AppendLine(@"
        <div class='contact-section'>
            <h3>פרטי קשר</h3>
            <div class='contact-info'>
                טלפון: 058-7818560 | אימייל: info@tryit.co.il
            </div>
            <p style='margin-top: 15px;'>תודה שבחרתם בנו!</p>
        </div>");

            // Footer
            html.AppendLine(@"
        <div class='footer'>
            <strong>הצעה תקפה ל-30 יום מיום הנפקתה</strong>
            <p style='margin-top: 10px;'>בברכה, צוות TRYIT</p>
        </div>
    </div>
</body>
</html>");

            return html.ToString();
        }

        private async Task<string> GetImageTag(string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    _logger.LogDebug("Empty or null image path provided");
                    return string.Empty;
                }

                // Check if it's an external URL
                if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(imagePath);
                    _logger.LogInformation("Using external URL for image from domain: {Domain}", uri.Host);
                    return $"<img src='{WebUtility.HtmlEncode(imagePath)}' alt='תמונת טיול' />";
                }

                // Handle local files - convert to base64
                var normalizedPath = imagePath.TrimStart('/');
                var fullPath = Path.Combine(_env.WebRootPath, normalizedPath);

                // Security: Validate the path to prevent directory traversal attacks
                var resolvedPath = Path.GetFullPath(fullPath);
                var webRootPath = Path.GetFullPath(_env.WebRootPath);

                if (!webRootPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    webRootPath += Path.DirectorySeparatorChar;
                }

                var normalizedResolvedPath = resolvedPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                var normalizedWebRootPath = webRootPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                if (!normalizedResolvedPath.StartsWith(normalizedWebRootPath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Path traversal attempt detected for: {RelativePath}", normalizedPath);
                    return string.Empty;
                }

                _logger.LogInformation("Attempting to load image: {RelativePath}", normalizedPath);

                if (!File.Exists(resolvedPath))
                {
                    _logger.LogWarning("Image file not found: {RelativePath}", normalizedPath);
                    return string.Empty;
                }

                var imageBytes = await File.ReadAllBytesAsync(resolvedPath);
                if (imageBytes.Length == 0)
                {
                    _logger.LogWarning("Image file is empty: {RelativePath}", normalizedPath);
                    return string.Empty;
                }

                var base64 = Convert.ToBase64String(imageBytes);
                var mimeType = GetMimeType(resolvedPath);

                _logger.LogInformation("Successfully loaded image: {RelativePath}, Size: {Size} bytes", normalizedPath, imageBytes.Length);
                return $"<img src='data:{mimeType};base64,{base64}' alt='תמונת טיול' />";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading image: {ImagePath}", imagePath);
                return string.Empty;
            }
        }

        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "image/jpeg"
            };
        }
    }
}
