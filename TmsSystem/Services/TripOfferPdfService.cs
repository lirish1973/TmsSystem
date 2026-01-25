using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;
using System.Web;
using TmsSystem.Models;

namespace TmsSystem.Services
{
    public class TripOfferPdfService : ITripOfferPdfService
    {
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment _env;

        public TripOfferPdfService(IConverter converter, IWebHostEnvironment env)
        {
            _converter = converter;
            _env = env;
        }

        public async Task<byte[]> GenerateTripOfferPdfAsync(TripOffer offer)
        {
            var html = await GenerateTripOfferHtmlAsync(offer);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return _converter.Convert(doc);
        }

        public async Task<string> GenerateTripOfferHtmlAsync(TripOffer offer)
        {
            var html = new StringBuilder();

            // Get ordered trip days
            var orderedDays = offer.Trip?.TripDays?.OrderBy(d => d.DayNumber).ToList() ?? new List<TripDay>();

            // HTML Header with RTL support
            html.AppendLine(@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>הצעת מחיר לטיול - TRYIT</title>
    <style>
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }
        body {
            font-family: 'Segoe UI', Tahoma, Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #f8f9fa;
            margin: 0;
            padding: 15px;
            direction: rtl;
            text-align: right;
        }
        .container {
            max-width: 900px;
            margin: 0 auto;
            background: white;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }
        .header h1 {
            margin: 0;
            font-size: 2.5em;
            font-weight: bold;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
        }
        .header p {
            margin: 10px 0 0 0;
            opacity: 0.95;
            font-size: 1.1em;
        }
        .logo-text {
            font-size: 3em;
            font-weight: bold;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }
        .content {
            padding: 30px;
            direction: rtl;
        }
        .section {
            margin-bottom: 25px;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 10px;
            border-right: 5px solid #667eea;
        }
        .section-title {
            font-size: 1.5em;
            font-weight: bold;
            color: #667eea;
            margin-bottom: 15px;
            text-align: right;
        }
        .info-row {
            margin-bottom: 12px;
            padding: 10px 0;
            border-bottom: 1px solid #e9ecef;
            display: flex;
            justify-content: space-between;
            align-items: center;
            direction: rtl;
        }
        .info-row:last-child {
            border-bottom: none;
        }
        .info-label {
            font-weight: bold;
            color: #495057;
            min-width: 140px;
            text-align: right;
        }
        .info-value {
            color: #212529;
            text-align: right;
            flex: 1;
        }
        .trip-day-card {
            background: white;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 10px;
            border-right: 5px solid #667eea;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            display: flex;
            gap: 20px;
            direction: rtl;
        }
        .trip-day-image {
            flex-shrink: 0;
            width: 250px;
            height: 180px;
            border-radius: 8px;
            overflow: hidden;
        }
        .trip-day-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }
        .trip-day-content {
            flex: 1;
            text-align: right;
        }
        .trip-day-number {
            background: #667eea;
            color: white;
            padding: 5px 15px;
            border-radius: 20px;
            font-weight: bold;
            display: inline-block;
            margin-bottom: 10px;
        }
        .trip-day-title {
            font-size: 1.3em;
            font-weight: bold;
            color: #333;
            margin-bottom: 8px;
        }
        .trip-day-location {
            color: #667eea;
            font-weight: bold;
            margin-bottom: 10px;
        }
        .trip-day-description {
            color: #6c757d;
            line-height: 1.6;
            text-align: right;
        }
        .price-section {
            background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
            color: white;
            border-right: 5px solid #28a745;
            border-radius: 10px;
            padding: 25px;
        }
        .price-section .section-title {
            color: white;
        }
        .price-section .info-label {
            color: rgba(255,255,255,0.95);
        }
        .price-section .info-value {
            color: white;
            font-weight: bold;
            font-size: 1.1em;
        }
        .price-highlight {
            font-size: 28px;
            font-weight: bold;
        }
        .includes-list, .excludes-list {
            list-style: none;
            padding: 0;
            direction: rtl;
        }
        .includes-list li {
            padding: 10px 0;
            border-bottom: 1px solid #e9ecef;
            position: relative;
            padding-right: 30px;
            text-align: right;
        }
        .includes-list li:before {
            content: '✓';
            position: absolute;
            right: 0;
            color: #28a745;
            font-weight: bold;
            font-size: 18px;
        }
        .excludes-list li {
            padding: 10px 0;
            border-bottom: 1px solid #e9ecef;
            position: relative;
            padding-right: 30px;
            text-align: right;
        }
        .excludes-list li:before {
            content: '✗';
            position: absolute;
            right: 0;
            color: #dc3545;
            font-weight: bold;
            font-size: 18px;
        }
        .terms-section {
            background: #e3f2fd;
            border-right: 5px solid #2196f3;
        }
        .term-item {
            margin-bottom: 12px;
            padding: 12px;
            background: white;
            border-radius: 8px;
            border-right: 3px solid #2196f3;
            line-height: 1.6;
        }
        .term-item strong {
            color: #2196f3;
        }
        .bank-details {
            background: #f0f9ff;
            padding: 20px;
            border-radius: 10px;
            border-right: 5px solid #0ea5e9;
            margin-top: 15px;
        }
        .contact-section {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            text-align: center;
            border-radius: 10px;
            padding: 25px;
        }
        .contact-section .section-title {
            color: white;
        }
        .footer {
            text-align: center;
            margin-top: 30px;
            padding: 20px;
            background: #fff3cd;
            border-radius: 10px;
            border: 2px solid #ffc107;
        }
        .footer strong {
            color: #856404;
            font-size: 1.1em;
        }
    </style>
</head>
<body>");

            // Build content
            html.AppendLine(@"
    <div class='container'>
        <div class='header'>
            <div class='logo-text'>TRYIT</div>
            <h1>הצעת מחיר לטיול</h1>
            <p>מספר הצעה: " + HttpUtility.HtmlEncode(offer.OfferNumber) + @"</p>
            <p>תאריך: " + offer.OfferDate.ToString("dd/MM/yyyy") + @"</p>
        </div>
        
        <div class='content'>");

            // Customer details
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>👤 פרטי הלקוח</div>
                <div class='info-row'>
                    <span class='info-label'>שם מלא:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(offer.Customer?.DisplayName ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>טלפון:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(offer.Customer?.Phone ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>אימייל:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(offer.Customer?.Email ?? "לא צוין") + @"</span>
                </div>");

            if (!string.IsNullOrEmpty(offer.Customer?.Address))
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>כתובת:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(offer.Customer.Address) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // Trip details
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>✈️ פרטי הטיול</div>
                <div class='info-row'>
                    <span class='info-label'>שם הטיול:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(offer.Trip?.Title ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>משך הטיול:</span>
                    <span class='info-value'>" + (offer.Trip?.NumberOfDays ?? 0) + @" ימים</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>תאריך יציאה:</span>
                    <span class='info-value'>" + offer.DepartureDate.ToString("dd/MM/yyyy") + @"</span>
                </div>");

            if (offer.ReturnDate.HasValue)
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>תאריך חזרה:</span>
                    <span class='info-value'>" + offer.ReturnDate.Value.ToString("dd/MM/yyyy") + @"</span>
                </div>");
            }

            html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>מספר משתתפים:</span>
                    <span class='info-value'>" + offer.Participants + @"</span>
                </div>
            </div>");

            // Trip description
            if (!string.IsNullOrEmpty(offer.Trip?.Description))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>📖 אודות הטיול</div>
                <p style='line-height: 1.8; text-align: right;'>" + HttpUtility.HtmlEncode(offer.Trip.Description).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
            </div>");
            }

            // Trip days with images
            if (orderedDays.Any())
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>🗓️ ימי הטיול</div>");

                foreach (var day in orderedDays)
                {
                    html.AppendLine(@"
                <div class='trip-day-card'>");

                    // Image section
                    if (!string.IsNullOrWhiteSpace(day.ImagePath))
                    {
                        var imageTag = await GetImageTag(day.ImagePath);
                        if (!string.IsNullOrEmpty(imageTag))
                        {
                            html.AppendLine(@"
                    <div class='trip-day-image'>" + imageTag + @"</div>");
                        }
                    }

                    // Content section
                    html.AppendLine(@"
                    <div class='trip-day-content'>
                        <span class='trip-day-number'>יום " + day.DayNumber + @"</span>
                        <div class='trip-day-title'>" + HttpUtility.HtmlEncode(day.Title) + @"</div>");

                    if (!string.IsNullOrWhiteSpace(day.Location))
                    {
                        html.AppendLine(@"
                        <div class='trip-day-location'>📍 " + HttpUtility.HtmlEncode(day.Location) + @"</div>");
                    }

                    if (!string.IsNullOrWhiteSpace(day.Description))
                    {
                        html.AppendLine(@"
                        <div class='trip-day-description'>" + HttpUtility.HtmlEncode(day.Description).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</div>");
                    }

                    html.AppendLine(@"
                    </div>
                </div>");
                }

                html.AppendLine("            </div>");
            }

            // What's included
            if (!string.IsNullOrEmpty(offer.Trip?.Includes))
            {
                var includesList = offer.Trip.Includes.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (includesList.Any())
                {
                    html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>✅ מה כלול במחיר</div>
                <ul class='includes-list'>");

                    foreach (var item in includesList)
                    {
                        html.AppendLine(@"
                    <li>" + HttpUtility.HtmlEncode(item.Trim()) + "</li>");
                    }

                    html.AppendLine(@"
                </ul>
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
                <div class='section-title'>❌ מה לא כלול במחיר</div>
                <ul class='excludes-list'>");

                    foreach (var item in excludesList)
                    {
                        html.AppendLine(@"
                    <li>" + HttpUtility.HtmlEncode(item.Trim()) + "</li>");
                    }

                    html.AppendLine(@"
                </ul>
            </div>");
                }
            }

            // Flight details
            if (offer.FlightIncluded && !string.IsNullOrEmpty(offer.FlightDetails))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>✈️ פרטי טיסה</div>
                <p style='line-height: 1.8; text-align: right;'>" + HttpUtility.HtmlEncode(offer.FlightDetails).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
            </div>");
            }

            // Special requests
            if (!string.IsNullOrEmpty(offer.SpecialRequests))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>📝 בקשות מיוחדות</div>
                <p style='line-height: 1.8; text-align: right;'>" + HttpUtility.HtmlEncode(offer.SpecialRequests).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
            </div>");
            }

            // Cancellation terms
            html.AppendLine(@"
            <div class='section terms-section'>
                <div class='section-title'>📋 תנאי ביטול ותשלום</div>
                <div class='term-item'>
                    <strong>תשלום מקדמה:</strong> על מנת לשריין את הטיול יש להעביר מקדמה של 50% עם אישור ההזמנה. יש לסיים את כל התשלום עד 7 ימים עסקים מיום יציאת הטיול.
                </div>
                <div class='term-item'>
                    <strong>דחיית תאריך:</strong> במידה ותרצו לדחות את התאריך עד 14 יום לפני הסיור ניתן לעשות זאת ללא עלות.
                </div>
                <div class='term-item'>
                    <strong>ביטול עד 30 יום:</strong> במידה ותרצו לבטל את הסיור עד 30 יום לפני הסיור, תקבלו החזר פחות 300ש״ח דמי טיפול.
                </div>
                <div class='term-item'>
                    <strong>ביטול 30-14 ימים:</strong> ביטול שיתקיים בין 30 יום ל 14 יום לפני מועד הסיור - יגבו דמי ביטול של 50% מהמחיר.
                </div>
                <div class='term-item'>
                    <strong>ביטול עד 14 ימים:</strong> ביטול שיתקיים בין 14 יום ליום הסיור - יגבו דמי ביטול מלאים.
                </div>
                <div class='term-item'>
                    <strong>ביטול מטעמנו:</strong> במידה ולא ניתן לקיים את הסיור, ואנו נבטל אותו בשל תנאים ביטחוניים או תנאי מזג אויר, ולא תרצו מועד חלופי - תשלום מלא יוחזר.
                </div>
            </div>");

            // Price breakdown
            html.AppendLine(@"
            <div class='section price-section'>
                <div class='section-title'>💰 פירוט מחירים</div>
                <div class='info-row'>
                    <span class='info-label'>מחיר לאדם:</span>
                    <span class='info-value price-highlight'>₪" + offer.PricePerPerson.ToString("N2") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>מספר משתתפים:</span>
                    <span class='info-value'>× " + offer.Participants + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>סכום ביניים:</span>
                    <span class='info-value'>₪" + (offer.PricePerPerson * offer.Participants).ToString("N2") + @"</span>
                </div>");

            if (offer.SingleRooms > 0 && offer.SingleRoomSupplement.HasValue)
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>תוספת חדרים יחידים (" + offer.SingleRooms + @" × ₪" + offer.SingleRoomSupplement.Value.ToString("N2") + @"):</span>
                    <span class='info-value'>₪" + (offer.SingleRoomSupplement.Value * offer.SingleRooms).ToString("N2") + @"</span>
                </div>");
            }

            if (offer.InsuranceIncluded && offer.InsurancePrice.HasValue)
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>ביטוח נסיעות (" + offer.Participants + @" × ₪" + offer.InsurancePrice.Value.ToString("N2") + @"):</span>
                    <span class='info-value'>₪" + (offer.InsurancePrice.Value * offer.Participants).ToString("N2") + @"</span>
                </div>");
            }

            html.AppendLine(@"
                <div class='info-row' style='border-top: 3px solid white; padding-top: 15px; margin-top: 10px;'>
                    <span class='info-label' style='font-size: 1.3em;'>סה""כ לתשלום:</span>
                    <span class='info-value price-highlight'>₪" + offer.TotalPrice.ToString("N2") + @"</span>
                </div>");

            // Payment method
            if (offer.PaymentMethod != null)
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>אמצעי תשלום:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(offer.PaymentMethod.PaymentName) + @"</span>
                </div>");

                if (offer.PaymentInstallments.HasValue)
                {
                    html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>מספר תשלומים:</span>
                    <span class='info-value'>" + offer.PaymentInstallments.Value + @" תשלומים</span>
                </div>");
                }
            }

            // Bank details
            html.AppendLine(@"
                <div class='bank-details'>
                    <div class='section-title' style='color: #0ea5e9; font-size: 1.3em;'>🏦 פרטי העברה בנקאית</div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #333;'>שם הבנק:</span>
                        <span class='info-value' style='color: #333;'>בנק לאומי</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #333;'>מספר סניף:</span>
                        <span class='info-value' style='color: #333;'>805</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #333;'>מספר חשבון:</span>
                        <span class='info-value' style='color: #333;'>39820047</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #333;'>שם בעל החשבון:</span>
                        <span class='info-value' style='color: #333;'>ספארי אפריקה בע״מ</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #333;'>ח.פ:</span>
                        <span class='info-value' style='color: #333;'>515323970</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #333; font-weight: bold;'>סכום להעברה:</span>
                        <span class='info-value' style='color: #28a745; font-weight: bold; font-size: 1.3em;'>₪" + offer.TotalPrice.ToString("N2") + @"</span>
                    </div>
                </div>
            </div>");

            // Contact info
            html.AppendLine(@"
            <div class='section contact-section'>
                <div class='section-title'>📞 פרטי קשר</div>
                <div style='font-size: 1.2em; margin-top: 15px;'>
                    <div style='margin-bottom: 10px;'>
                        <strong>טלפון:</strong> 058-7818560
                    </div>
                    <div style='margin-bottom: 10px;'>
                        <strong>אימייל:</strong> info@tryit.co.il
                    </div>
                    <div style='margin-top: 20px; font-size: 1.3em;'>
                        תודה שבחרתם בנו! 🌟
                    </div>
                </div>
            </div>");

            // Footer - validity
            html.AppendLine(@"
            <div class='footer'>
                <strong>הצעה תקפה ל-30 יום מיום הנפקתה</strong>
                <p style='margin-top: 10px; color: #856404;'>בברכה, צוות TRYIT</p>
            </div>
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
                // Check if it's an external URL
                if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return $"<img src='{HttpUtility.HtmlEncode(imagePath)}' alt='תמונת טיול' />";
                }

                // Handle local files - convert to base64
                if (imagePath.StartsWith("/"))
                {
                    imagePath = imagePath.TrimStart('/');
                }

                var fullPath = Path.Combine(_env.WebRootPath, imagePath);

                if (File.Exists(fullPath))
                {
                    var imageBytes = await File.ReadAllBytesAsync(fullPath);
                    var base64 = Convert.ToBase64String(imageBytes);
                    var mimeType = GetMimeType(fullPath);
                    return $"<img src='data:{mimeType};base64,{base64}' alt='תמונת טיול' />";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image {imagePath}: {ex.Message}");
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
