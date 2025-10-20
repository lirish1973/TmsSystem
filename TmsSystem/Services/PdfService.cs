using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System.Text;
using TmsSystem.ViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Web;

namespace TmsSystem.Services
{
    public class PdfService : IPdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public async Task<byte[]> GenerateOfferPdfAsync(ShowOfferViewModel model)
        {
            var html = await GenerateOfferHtmlAsync(model);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 20, Right = 20 }
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

        public async Task<string> GenerateOfferHtmlAsync(ShowOfferViewModel model)
        {
            var html = new StringBuilder();

            // HTML Header with RTL support
            html.AppendLine(@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>הצעת מחיר - TRYIT System</title>
    <style>
        * {
            box-sizing: border-box;
        }
        body {
            font-family: 'Segoe UI', Tahoma, Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #f8f9fa;
            margin: 0;
            padding: 20px;
            direction: rtl;
            text-align: right;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
            background: white;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        .header {
            background: linear-gradient(135deg, #1e3a8a 0%, #1e40af 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }
        .header h1 {
            margin: 0;
            font-size: 2.2em;
            font-weight: bold;
        }
        .header p {
            margin: 10px 0 0 0;
            opacity: 0.9;
            font-size: 1.1em;
        }
        .content {
            padding: 30px;
            direction: rtl;
        }
        .section {
            margin-bottom: 25px;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 8px;
            border-right: 4px solid #1e3a8a;
        }
        .section-title {
            font-size: 1.4em;
            font-weight: bold;
            color: #1e3a8a;
            margin-bottom: 15px;
            text-align: right;
        }
        .info-row {
            margin-bottom: 12px;
            padding: 8px 0;
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
            min-width: 120px;
            text-align: right;
        }
        .info-value {
            color: #212529;
            text-align: right;
            flex: 1;
        }
        .price-section {
            background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
            color: white;
            border-right: 4px solid #28a745;
        }
        .price-section .section-title {
            color: white;
        }
        .price-section .info-label {
            color: rgba(255,255,255,0.9);
        }
        .price-section .info-value {
            color: white;
            font-weight: bold;
        }
        .price-highlight {
            font-size: 24px;
            font-weight: bold;
        }
        .schedule-item {
            background: white;
            padding: 15px;
            margin-bottom: 15px;
            border-radius: 8px;
            border-right: 3px solid #1e3a8a;
            direction: rtl;
        }
        .schedule-time {
            font-weight: bold;
            color: #1e3a8a;
            font-size: 16px;
            margin-bottom: 8px;
            text-align: right;
        }
        .schedule-location {
            font-weight: bold;
            color: #495057;
            margin-bottom: 5px;
            text-align: right;
        }
        .schedule-description {
            color: #6c757d;
            font-size: 14px;
            line-height: 1.5;
            text-align: right;
        }
        .includes-list, .excludes-list {
            list-style: none;
            padding: 0;
            direction: rtl;
        }
        .includes-list li {
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
            position: relative;
            padding-right: 25px;
            text-align: right;
        }
        .includes-list li:before {
            content: '✓';
            position: absolute;
            right: 0;
            color: #28a745;
            font-weight: bold;
        }
        .excludes-list li {
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
            position: relative;
            padding-right: 25px;
            text-align: right;
        }
        .excludes-list li:before {
            content: '✗';
            position: absolute;
            right: 0;
            color: #dc3545;
            font-weight: bold;
        }
        .terms-section {
            background: #e3f2fd;
            border-right: 4px solid #2196f3;
        }
        .term-item {
            margin-bottom: 15px;
            padding: 12px;
            background: white;
            border-radius: 8px;
            border-right: 3px solid #1e3a8a;
            line-height: 1.6;
        }
        .term-item strong {
            color: #1e3a8a;
        }
        .bank-details {
            background: #f0f9ff;
            padding: 20px;
            border-radius: 8px;
            border-right: 4px solid #0ea5e9;
            margin-top: 15px;
        }
        .bank-details .section-title {
            color: #212529 !important;
        }
        .bank-details .info-label {
            color: #212529 !important;
        }
        .bank-details .info-value {
            color: #212529 !important;
        }
        .bank-details .price-highlight {
            color: #28a745 !important;
            font-weight: bold;
        }
        .contact-info {
            background: #f3e5f5;
            border-right: 4px solid #9c27b0;
            text-align: center;
            color: #212529 !important;
        }
        .contact-info .section-title {
            color: #212529 !important;
        }
        .contact-info .info-label {
            color: #212529 !important;
        }
        .contact-info .info-value {
            color: #212529 !important;
        }
        .logo-container {
            margin-bottom: 20px;
            text-align: center;
        }
        .company-logo {
            max-width: 150px;
            max-height: 80px;
            width: auto;
            height: auto;
            object-fit: contain;
        }
    </style>
</head>
<body>");

            // בניית התוכן
            html.AppendLine(@"
    <div class='container'>
        <div class='header'>
            <div class='logo-container'>
                <img alt='לוגו TRYIT' class='company-logo' src='https://www.tryit.co.il/wp-content/uploads/2025/08/cropped-try-it-logo.png' />
            </div>
            <h1>מערכת ניהול הצעות מחיר - TRYIT</h1>
            <p>הצעת מחיר מספר " + model.Offer.OfferId + @"</p>
            <p>נוצרה: " + model.Offer.CreatedAt.ToString("dd/MM/yyyy HH:mm") + @"</p>
        </div>
        
        <div class='content'>");

            // פרטי הלקוח
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>פרטי הלקוח</div>
                <div class='info-row'>
                    <span class='info-label'>שם מלא:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Customer?.FullName ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>טלפון:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Customer?.Phone ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>אימייל:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Customer?.Email ?? "לא צוין") + @"</span>
                </div>");

            if (!string.IsNullOrEmpty(model.Offer.Customer?.Address))
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>כתובת:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Customer.Address) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // פרטי הטיול
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>פרטי הטיול</div>
                <div class='info-row'>
                    <span class='info-label'>טיול:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Tour?.Title ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>תאריך:</span>
                    <span class='info-value'>" + model.Offer.TourDate.ToString("dd/MM/yyyy") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>משתתפים:</span>
                    <span class='info-value'>" + model.Offer.Participants + @"</span>
                </div>");

            if (!string.IsNullOrEmpty(model.Offer.PickupLocation))
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>נקודת איסוף:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.PickupLocation) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // פרטי המדריך
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>פרטי המדריך</div>
                <div class='info-row'>
                    <span class='info-label'>שם המדריך:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Guide?.GuideName ?? "לא צוין") + @"</span>
                </div>");

            if (!string.IsNullOrEmpty(model.Offer.Guide?.Description))
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>תיאור:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Guide.Description) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // תיאור הטיול
            if (!string.IsNullOrEmpty(model.Offer.Tour?.Description))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>אודות הסיור - " + HttpUtility.HtmlEncode(model.Offer.Tour.Title) + @"</div>
                <p>" + HttpUtility.HtmlEncode(model.Offer.Tour.Description).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
            </div>");
            }

            // לוח זמנים מפורט
            if (model.Offer.Tour?.Schedule != null && model.Offer.Tour.Schedule.Any())
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>לוח זמנים מפורט</div>");

                foreach (var item in model.Offer.Tour.Schedule.OrderBy(s => s.StartTime))
                {
                    html.AppendLine(@"
                <div class='schedule-item'>
                    <div class='schedule-time'>" + item.StartTime.ToString(@"hh\:mm"));

                    if (item.EndTime.HasValue)
                    {
                        html.AppendLine(@" - " + item.EndTime.Value.ToString(@"hh\:mm"));
                    }

                    html.AppendLine(@"</div>
                    <div class='schedule-location'>" + HttpUtility.HtmlEncode(item.Location ?? "") + @"</div>
                    <div class='schedule-description'>" + HttpUtility.HtmlEncode(item.Description ?? "").Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</div>
                </div>");
                }

                html.AppendLine("            </div>");
            }

            // מה כלול בסיור
            if (model.Offer.Tour?.Includes != null && model.Offer.Tour.Includes.Any())
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>הסיור כולל</div>
                <ul class='includes-list'>");

                foreach (var include in model.Offer.Tour.Includes)
                {
                    html.AppendLine(@"
                    <li>" + HttpUtility.HtmlEncode(include.Description ?? include.Text ?? "") + "</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>");
            }

            // מה לא כלול בסיור
            if (model.Offer.Tour?.Excludes != null && model.Offer.Tour.Excludes.Any())
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>הסיור לא כולל</div>
                <ul class='excludes-list'>");

                foreach (var exclude in model.Offer.Tour.Excludes)
                {
                    html.AppendLine(@"
                    <li>" + HttpUtility.HtmlEncode(exclude.Description ?? exclude.Text ?? "") + "</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>");
            }

            // מה כלול במחיר
            if (!string.IsNullOrEmpty(model.Offer.PriceIncludes))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>המחיר כולל</div>
                <ul class='includes-list'>");

                var priceIncludes = model.Offer.PriceIncludes.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in priceIncludes)
                {
                    html.AppendLine(@"
                    <li>" + HttpUtility.HtmlEncode(item.Trim()) + "</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>");
            }

            // מה לא כלול במחיר
            if (!string.IsNullOrEmpty(model.Offer.PriceExcludes))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>המחיר אינו כולל</div>
                <ul class='excludes-list'>");

                var priceExcludes = model.Offer.PriceExcludes.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in priceExcludes)
                {
                    html.AppendLine(@"
                    <li>" + HttpUtility.HtmlEncode(item.Trim()) + "</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>");
            }

            // בקשות מיוחדות
            if (!string.IsNullOrEmpty(model.Offer.SpecialRequests))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>בקשות מיוחדות וטקסים</div>
                <div>" + HttpUtility.HtmlEncode(model.Offer.SpecialRequests).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</div>
            </div>");
            }

            // תנאי ביטול ותשלום
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

            // פרטי מחיר
            html.AppendLine(@"
            <div class='section price-section'>
                <div class='section-title'>פרטי מחיר</div>
                <div class='info-row'>
                    <span class='info-label'>מחיר לאדם:</span>
                    <span class='info-value price-highlight'>₪" + model.Offer.Price.ToString("N0") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>סה״כ לתשלום:</span>
                    <span class='info-value price-highlight'>₪" + model.Offer.TotalPayment.ToString("N0") + @"</span>
                </div>");

            if (model.PaymentMethod != null)
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>שיטת תשלום:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.PaymentMethod.METHOD) + @"</span>
                </div>");
            }

            html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>ארוחת צהריים:</span>
                    <span class='info-value'>" + (model.Offer.LunchIncluded ? "כלולה במחיר" : "לא כלולה") + @"</span>
                </div>");

            // פרטי העברה בנקאית
            html.AppendLine(@"
                <div class='bank-details'>
                    <div class='section-title' style='color: #212529;'>פרטי העברה בנקאית</div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #212529;'>שם הבנק:</span>
                        <span class='info-value' style='color: #212529;'>בנק לאומי</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #212529;'>מספר סניף:</span>
                        <span class='info-value' style='color: #212529;'>805</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #212529;'>מספר חשבון:</span>
                        <span class='info-value' style='color: #212529;'>39820047</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #212529;'>שם בעל החשבון:</span>
                        <span class='info-value' style='color: #212529;'>ספארי אפריקה בע״מ</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #212529;'>ח.פ:</span>
                        <span class='info-value' style='color: #212529;'>515323970</span>
                    </div>
                    <div class='info-row'>
                        <span class='info-label' style='color: #212529;'>סכום להעברה:</span>
                        <span class='info-value price-highlight' style='color: #28a745; font-weight: bold;'>₪" + model.Offer.TotalPayment.ToString("N0") + @"</span>
                    </div>
                </div>");

            // פרטי קשר
            html.AppendLine(@"
            <div class='section contact-info'>
                <div class='section-title' style='color: #212529;'>פרטי קשר</div>
                <div class='info-row'>
                    <span class='info-label' style='color: #212529;'>טלפון: 📞</span>
                    <span class='info-value' style='color: #212529;'>058-7818560</span>
                </div>
                <div class='info-row'>
                    <span class='info-label' style='color: #212529;'>אימייל: 📧</span>
                    <span class='info-value' style='color: #212529;'>info@tryit.co.il</span>
                </div>
                <div style='margin-top: 20px; font-weight: bold; color: #212529;'>
                    תודה שבחרתם בנו!
                </div>
            </div>");

            // הודעה על תוקף ההצעה
            html.AppendLine(@"
            <div style='text-align: center; margin-top: 30px; padding: 15px; background: #fff3cd; border-radius: 8px; border: 1px solid #ffeaa7;'>
                <div class='logo-container'>
                    <img alt='לוגו TRYIT' class='company-logo' src='https://www.tryit.co.il/wp-content/uploads/2025/08/cropped-try-it-logo.png' />
                </div>
                <strong style='color: #856404;'>הצעה תקפה ל-30 יום</strong>
            </div>");

            // סיום
            html.AppendLine(@"
        </div>
    </div>
</body>
</html>");

            return html.ToString();
        }

        // פונקציות עזר
        private string ReverseHebrewSafely(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            bool hasHebrew = text.Any(c => c >= '\u0590' && c <= '\u05FF');
            if (!hasHebrew)
                return text;

            var parts = text.Split(' ');
            var processedParts = new List<string>();

            foreach (var part in parts)
            {
                bool partHasHebrew = part.Any(c => c >= '\u0590' && c <= '\u05FF');

                if (partHasHebrew)
                {
                    var chars = part.ToCharArray();
                    Array.Reverse(chars);
                    processedParts.Add(new string(chars));
                }
                else
                {
                    processedParts.Add(part);
                }
            }

            processedParts.Reverse();
            return string.Join(" ", processedParts);
        }

        private string ReverseHebrewCharsOnly(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var chars = text.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        private async Task<string> GetLogoBase64()
        {
            try
            {
                // החזר בסיס64 של הלוגו או string ריק
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error loading logo: {ex.Message}");
                return "";
            }
        }
    }
}