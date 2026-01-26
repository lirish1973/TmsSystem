using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System.Text;
using TmsSystem.ViewModels;
using System.Web;
using iText.Html2pdf;
using iText.Layout.Font;
using iText.IO.Font;

namespace TmsSystem.Services
{
    public class PdfService : IPdfService
    {
        // Unicode BiDi control characters for RTL text
        private const char RLE = '\u202B'; // RIGHT-TO-LEFT EMBEDDING - marks start of RTL text
        private const char PDF = '\u202C'; // POP DIRECTIONAL FORMATTING - marks end of RTL text

        public PdfService()
        {
            // Hebrew RTL text support is handled via proper HTML structure (dir='rtl', lang='he')
            // and CSS directives (direction: rtl, text-align: right, unicode-bidi: embed)
            // Additional Unicode BiDi markers ensure proper text direction in PDF
        }

        /// <summary>
        /// Wraps Hebrew/RTL text with Unicode BiDi markers to ensure proper rendering in PDF
        /// </summary>
        private string WrapRtlText(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            
            // Check if text contains Hebrew characters
            bool hasHebrew = text.Any(c => c >= 0x0590 && c <= 0x05FF);
            
            if (hasHebrew)
            {
                // Wrap with RLE...PDF to force RTL rendering
                return $"{RLE}{text}{PDF}";
            }
            
            return text;
        }

        /// <summary>
        /// HTML-encodes and wraps RTL text for safe use in PDF generation
        /// </summary>
        private string EncodeAndWrapRtl(string? text, string defaultValue = "לא צוין")
        {
            var textToUse = text ?? defaultValue;
            var encoded = HttpUtility.HtmlEncode(textToUse);
            return WrapRtlText(encoded);
        }

        public async Task<byte[]> GenerateOfferPdfAsync(ShowOfferViewModel model)
        {
            var html = await GenerateOfferHtmlAsync(model);

            using var memoryStream = new MemoryStream();
            
            try
            {
                // Configure converter properties for Hebrew support
                var converterProperties = new ConverterProperties();
                converterProperties.SetCharset("UTF-8");
                
                // Set base URI for resolving external resources like images
                converterProperties.SetBaseUri("https://www.tryit.co.il/");
                
                // Add font provider for better Hebrew and RTL support
                var fontProvider = new FontProvider();
                fontProvider.AddStandardPdfFonts();
                fontProvider.AddSystemFonts();
                converterProperties.SetFontProvider(fontProvider);
                
                HtmlConverter.ConvertToPdf(html, memoryStream, converterProperties);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow with more context
                throw new InvalidOperationException($"Failed to generate PDF: {ex.Message}", ex);
            }
            
            return memoryStream.ToArray();
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
            unicode-bidi: embed;
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
            min-width: 140px;
            text-align: right;
            font-size: 1.05em;
        }
        .info-value {
            color: #2d3748;
            text-align: right;
            flex: 1;
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
        .schedule-item {
            background: white;
            padding: 18px;
            margin-bottom: 18px;
            border-radius: 10px;
            border-right: 4px solid #667eea;
            direction: rtl;
            box-shadow: 0 2px 6px rgba(0,0,0,0.08);
        }
        .schedule-time {
            font-weight: 700;
            color: #667eea;
            font-size: 1.1em;
            margin-bottom: 10px;
            text-align: right;
        }
        .schedule-location {
            font-weight: 600;
            color: #4a5568;
            margin-bottom: 8px;
            text-align: right;
            font-size: 1.05em;
        }
        .schedule-description {
            color: #718096;
            font-size: 1.05em;
            line-height: 1.8;
            text-align: right;
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
        .bank-details .price-highlight {
            color: #10b981;
            font-weight: 900;
        }
        .contact-info {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-right: 6px solid #667eea;
            text-align: center;
            color: white;
            border-radius: 12px;
            padding: 30px;
            box-shadow: 0 4px 12px rgba(102,126,234,0.3);
        }
        .contact-info .section-title {
            color: white;
            border-bottom: 2px solid rgba(255,255,255,0.3);
        }
        .contact-info .info-label {
            color: white;
            font-weight: 600;
        }
        .contact-info .info-value {
            color: white;
            font-weight: 600;
        }
        .logo-container {
            margin-bottom: 20px;
            text-align: center;
        }
        .company-logo {
            max-width: 180px;
            max-height: 100px;
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
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.Customer?.FullName) + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>טלפון:</span>
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.Customer?.Phone) + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>אימייל:</span>
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.Customer?.Email) + @"</span>
                </div>");

            if (!string.IsNullOrEmpty(model.Offer.Customer?.Address))
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>כתובת:</span>
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.Customer.Address) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // פרטי הטיול
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>פרטי הטיול</div>
                <div class='info-row'>
                    <span class='info-label'>טיול:</span>
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.Tour?.Title) + @"</span>
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
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.PickupLocation) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // פרטי המדריך
            html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>פרטי המדריך</div>
                <div class='info-row'>
                    <span class='info-label'>שם המדריך:</span>
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.Guide?.GuideName) + @"</span>
                </div>");

            if (!string.IsNullOrEmpty(model.Offer.Guide?.Description))
            {
                html.AppendLine(@"
                <div class='info-row'>
                    <span class='info-label'>תיאור:</span>
                    <span class='info-value'>" + EncodeAndWrapRtl(model.Offer.Guide.Description) + @"</span>
                </div>");
            }

            html.AppendLine("            </div>");

            // תיאור הטיול
            if (!string.IsNullOrEmpty(model.Offer.Tour?.Description))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>אודות הסיור - " + EncodeAndWrapRtl(model.Offer.Tour.Title) + @"</div>
                <p>" + EncodeAndWrapRtl(model.Offer.Tour.Description).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</p>
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
                    <div class='schedule-location'>" + EncodeAndWrapRtl(item.Location, "") + @"</div>
                    <div class='schedule-description'>" + EncodeAndWrapRtl(item.Description, "").Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</div>
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
                    <li>" + EncodeAndWrapRtl(include.Description ?? include.Text, "") + "</li>");
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
                    <li>" + EncodeAndWrapRtl(exclude.Description ?? exclude.Text, "") + "</li>");
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
                    <li>" + EncodeAndWrapRtl(item.Trim()) + "</li>");
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
                    <li>" + EncodeAndWrapRtl(item.Trim()) + "</li>");
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
                <div>" + EncodeAndWrapRtl(model.Offer.SpecialRequests).Replace("\n", "<br/>").Replace("\r\n", "<br/>") + @"</div>
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
                    <span class='info-value'>" + EncodeAndWrapRtl(model.PaymentMethod.METHOD) + @"</span>
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
    }
}