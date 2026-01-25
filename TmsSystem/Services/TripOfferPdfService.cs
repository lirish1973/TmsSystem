using System.Text;
using System.Net;
using TmsSystem.Models;
using iText.Html2pdf;
using iText.Layout.Font;
using iText.IO.Font;

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

        public async Task<byte[]> GenerateTripOfferPdfAsync(TripOffer offer)
        {
            var html = await GenerateTripOfferHtmlAsync(offer);

            using var memoryStream = new MemoryStream();
            
            // Configure converter properties for Hebrew support
            var converterProperties = new ConverterProperties();
            converterProperties.SetCharset("UTF-8");
            
            // Add font provider for better Hebrew and RTL support
            var fontProvider = new FontProvider();
            fontProvider.AddStandardPdfFonts();
            fontProvider.AddSystemFonts();
            converterProperties.SetFontProvider(fontProvider);
            
            HtmlConverter.ConvertToPdf(html, memoryStream, converterProperties);
            
            return memoryStream.ToArray();
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
            font-family: 'Segoe UI', Tahoma, Geneva, Arial, sans-serif;
            line-height: 1.8;
            color: #2d3748;
            background: linear-gradient(to bottom, #f7fafc 0%, #edf2f7 100%);
            margin: 0;
            padding: 20px;
            direction: rtl;
            text-align: right;
        }
        .container {
            max-width: 900px;
            margin: 0 auto;
            background: white;
            border-radius: 16px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.12);
            overflow: hidden;
        }
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 50px 40px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }
        .header h1 {
            margin: 0;
            font-size: 2.8em;
            font-weight: 700;
            text-shadow: 2px 4px 8px rgba(0,0,0,0.3);
            letter-spacing: 1px;
        }
        .header p {
            margin: 15px 0 0 0;
            opacity: 0.95;
            font-size: 1.2em;
            font-weight: 300;
        }
        .logo-text {
            font-size: 3.5em;
            font-weight: 900;
            margin-bottom: 15px;
            text-shadow: 3px 3px 10px rgba(0,0,0,0.4);
            letter-spacing: 3px;
            background: linear-gradient(45deg, #fff, #f0f0f0);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }
        .content {
            padding: 40px;
            direction: rtl;
        }
        .section {
            margin-bottom: 30px;
            padding: 25px;
            background: linear-gradient(to bottom, #f9fafb 0%, #ffffff 100%);
            border-radius: 12px;
            border-right: 6px solid #667eea;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            transition: transform 0.2s;
        }
        .section:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        }
        .section-title {
            font-size: 1.6em;
            font-weight: 700;
            color: #667eea;
            margin-bottom: 20px;
            text-align: right;
            padding-bottom: 10px;
            border-bottom: 2px solid #e2e8f0;
        }
        .info-row {
            margin-bottom: 14px;
            padding: 12px 0;
            border-bottom: 1px solid #e2e8f0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            direction: rtl;
        }
        .info-row:last-child {
            border-bottom: none;
        }
        .info-label {
            font-weight: 600;
            color: #4a5568;
            min-width: 150px;
            text-align: right;
            font-size: 1.05em;
        }
        .info-value {
            color: #2d3748;
            text-align: right;
            flex: 1;
            font-size: 1.05em;
        }
        .trip-day-card {
            background: white;
            padding: 25px;
            margin-bottom: 25px;
            border-radius: 12px;
            border-right: 6px solid #667eea;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            display: flex;
            gap: 25px;
            direction: rtl;
            transition: all 0.3s ease;
        }
        .trip-day-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 8px 20px rgba(0,0,0,0.12);
        }
        .trip-day-image {
            flex-shrink: 0;
            width: 280px;
            height: 200px;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }
        .trip-day-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            transition: transform 0.3s ease;
        }
        .trip-day-image img:hover {
            transform: scale(1.05);
        }
        .trip-day-content {
            flex: 1;
            text-align: right;
        }
        .trip-day-number {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 6px 18px;
            border-radius: 25px;
            font-weight: 700;
            display: inline-block;
            margin-bottom: 12px;
            box-shadow: 0 2px 6px rgba(102,126,234,0.4);
            font-size: 0.95em;
        }
        .trip-day-title {
            font-size: 1.4em;
            font-weight: 700;
            color: #2d3748;
            margin-bottom: 10px;
            line-height: 1.4;
        }
        .trip-day-location {
            color: #667eea;
            font-weight: 600;
            margin-bottom: 12px;
            font-size: 1.1em;
        }
        .trip-day-description {
            color: #718096;
            line-height: 1.8;
            text-align: right;
            font-size: 1.05em;
        }
        .price-section {
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: white;
            border-right: 6px solid #047857;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 4px 12px rgba(16,185,129,0.3);
        }
        .price-section .section-title {
            color: white;
            border-bottom: 2px solid rgba(255,255,255,0.3);
        }
        .price-section .info-label {
            color: rgba(255,255,255,0.95);
            font-weight: 600;
        }
        .price-section .info-value {
            color: white;
            font-weight: 700;
            font-size: 1.15em;
        }
        .price-highlight {
            font-size: 32px;
            font-weight: 900;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
        }
        .includes-list, .excludes-list {
            list-style: none;
            padding: 0;
            direction: rtl;
        }
        .includes-list li {
            padding: 12px 0;
            border-bottom: 1px solid #e2e8f0;
            position: relative;
            padding-right: 35px;
            text-align: right;
            font-size: 1.05em;
            line-height: 1.6;
        }
        .includes-list li:before {
            content: '✓';
            position: absolute;
            right: 0;
            color: #10b981;
            font-weight: bold;
            font-size: 20px;
        }
        .excludes-list li {
            padding: 12px 0;
            border-bottom: 1px solid #e2e8f0;
            position: relative;
            padding-right: 35px;
            text-align: right;
            font-size: 1.05em;
            line-height: 1.6;
        }
        .excludes-list li:before {
            content: '✗';
            position: absolute;
            right: 0;
            color: #ef4444;
            font-weight: bold;
            font-size: 20px;
        }
        .terms-section {
            background: linear-gradient(to bottom, #dbeafe 0%, #e0f2fe 100%);
            border-right: 6px solid #3b82f6;
        }
        .term-item {
            margin-bottom: 15px;
            padding: 15px;
            background: white;
            border-radius: 10px;
            border-right: 4px solid #3b82f6;
            line-height: 1.8;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
            font-size: 1.05em;
        }
        .term-item strong {
            color: #1d4ed8;
            font-size: 1.1em;
        }
        .bank-details {
            background: linear-gradient(to bottom, #f0f9ff 0%, #e0f2fe 100%);
            padding: 25px;
            border-radius: 12px;
            border-right: 6px solid #0ea5e9;
            margin-top: 20px;
            box-shadow: 0 2px 8px rgba(14,165,233,0.1);
        }
        .bank-details .section-title {
            color: #0c4a6e;
            font-size: 1.4em;
        }
        .bank-details .info-label {
            color: #1e293b;
            font-weight: 600;
        }
        .bank-details .info-value {
            color: #0f172a;
            font-weight: 600;
        }
        .contact-section {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            text-align: center;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 4px 12px rgba(102,126,234,0.3);
        }
        .contact-section .section-title {
            color: white;
            border-bottom: 2px solid rgba(255,255,255,0.3);
        }
        .footer {
            text-align: center;
            margin-top: 35px;
            padding: 25px;
            background: linear-gradient(to bottom, #fef3c7 0%, #fde68a 100%);
            border-radius: 12px;
            border: 3px solid #f59e0b;
            box-shadow: 0 4px 8px rgba(245,158,11,0.2);
        }
        .footer strong {
            color: #92400e;
            font-size: 1.2em;
            font-weight: 700;
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
            <p>מספר הצעה: " + WebUtility.HtmlEncode(offer.OfferNumber) + @"</p>
            <p>תאריך: " + offer.OfferDate.ToString("dd/MM/yyyy") + @"</p>
        </div>
        
        <div class='content'>");

            // Customer details
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>👤 פרטי הלקוח</div>
                <div class='info-row'>
                    <span class='info-label'>שם מלא:</span>
                    <span class='info-value'>" + WebUtility.HtmlEncode(offer.Customer?.DisplayName ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>טלפון:</span>
                    <span class='info-value'>" + WebUtility.HtmlEncode(offer.Customer?.Phone ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>אימייל:</span>
                    <span class='info-value'>" + WebUtility.HtmlEncode(offer.Customer?.Email ?? "לא צוין") + @"</span>
                </div>");

            if (!string.IsNullOrEmpty(offer.Customer?.Address))
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>כתובת:</span>
                    <span class='info-value'>" + WebUtility.HtmlEncode(offer.Customer.Address) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // Trip details
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>✈️ פרטי הטיול</div>
                <div class='info-row'>
                    <span class='info-label'>שם הטיול:</span>
                    <span class='info-value'>" + WebUtility.HtmlEncode(offer.Trip?.Title ?? "לא צוין") + @"</span>
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
                <p style='line-height: 1.8; text-align: right;'>" + WebUtility.HtmlEncode(offer.Trip.Description).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
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
                        <div class='trip-day-title'>" + WebUtility.HtmlEncode(day.Title) + @"</div>");

                    if (!string.IsNullOrWhiteSpace(day.Location))
                    {
                        html.AppendLine(@"
                        <div class='trip-day-location'>📍 " + WebUtility.HtmlEncode(day.Location) + @"</div>");
                    }

                    if (!string.IsNullOrWhiteSpace(day.Description))
                    {
                        html.AppendLine(@"
                        <div class='trip-day-description'>" + WebUtility.HtmlEncode(day.Description).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</div>");
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
                    <li>" + WebUtility.HtmlEncode(item.Trim()) + "</li>");
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
                    <li>" + WebUtility.HtmlEncode(item.Trim()) + "</li>");
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
                <p style='line-height: 1.8; text-align: right;'>" + WebUtility.HtmlEncode(offer.FlightDetails).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
            </div>");
            }

            // Special requests
            if (!string.IsNullOrEmpty(offer.SpecialRequests))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>📝 בקשות מיוחדות</div>
                <p style='line-height: 1.8; text-align: right;'>" + WebUtility.HtmlEncode(offer.SpecialRequests).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
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
                    <span class='info-value'>" + WebUtility.HtmlEncode(offer.PaymentMethod.PaymentName) + @"</span>
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
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    _logger.LogWarning("Empty or null image path provided");
                    return string.Empty;
                }

                // Check if it's an external URL
                if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Using external URL for image: {ImagePath}", imagePath);
                    return $"<img src='{WebUtility.HtmlEncode(imagePath)}' alt='תמונת טיול' />";
                }

                // Handle local files - convert to base64
                var normalizedPath = imagePath.TrimStart('/');
                var fullPath = Path.Combine(_env.WebRootPath, normalizedPath);
                
                _logger.LogInformation("Attempting to load image from: {FullPath}", fullPath);

                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("Image file not found: {FullPath}", fullPath);
                    return string.Empty;
                }

                var imageBytes = await File.ReadAllBytesAsync(fullPath);
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    _logger.LogWarning("Image file is empty: {FullPath}", fullPath);
                    return string.Empty;
                }

                var base64 = Convert.ToBase64String(imageBytes);
                var mimeType = GetMimeType(fullPath);
                
                _logger.LogInformation("Successfully loaded image: {FullPath}, Size: {Size} bytes", fullPath, imageBytes.Length);
                return $"<img src='data:{mimeType};base64,{base64}' alt='תמונת טיול' />";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading image {ImagePath}", imagePath);
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
