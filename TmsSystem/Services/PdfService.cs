using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System.Text;
using TmsSystem.ViewModels;
using System.Web;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace TmsSystem.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
        }

        /// <summary>
        /// HTML-encodes text for safe use in PDF generation
        /// </summary>
        private string EncodeHtml(string? text, string defaultValue = "לא צוין")
        {
            var textToUse = text ?? defaultValue;
            return HttpUtility.HtmlEncode(textToUse);
        }

        public async Task<byte[]> GenerateOfferPdfAsync(ShowOfferViewModel model)
        {
            var html = await GenerateOfferHtmlAsync(model);

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
                throw new InvalidOperationException($"Failed to generate PDF: {ex.Message}", ex);
            }
        }

        public async Task<string> GenerateOfferHtmlAsync(ShowOfferViewModel model)
        {
            var html = new StringBuilder();

            // Professional HTML with proper RTL support
            html.AppendLine(@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <title>הצעת מחיר - TRYIT</title>
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

        /* Schedule */
        .schedule-item {
            background: #f8fafc;
            padding: 12px;
            margin-bottom: 10px;
            border-radius: 4px;
            border-right: 3px solid #1a365d;
        }

        .schedule-time {
            font-weight: 600;
            color: #1a365d;
            margin-bottom: 5px;
        }

        .schedule-location {
            font-weight: 500;
            color: #4a5568;
            margin-bottom: 5px;
        }

        .schedule-description {
            color: #718096;
            font-size: 13px;
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
            <h1>הצעת מחיר</h1>
            <div class='offer-info'>
                הצעה מספר: {model.Offer.OfferId} | תאריך: {model.Offer.CreatedAt:dd/MM/yyyy HH:mm}
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
                        <td class='info-value'>{EncodeHtml(model.Offer.Customer?.FullName)}</td>
                    </tr>
                    <tr>
                        <td class='info-label'>טלפון:</td>
                        <td class='info-value'>{EncodeHtml(model.Offer.Customer?.Phone)}</td>
                    </tr>
                    <tr>
                        <td class='info-label'>אימייל:</td>
                        <td class='info-value'>{EncodeHtml(model.Offer.Customer?.Email)}</td>
                    </tr>");

            if (!string.IsNullOrEmpty(model.Offer.Customer?.Address))
            {
                html.AppendLine($@"
                    <tr>
                        <td class='info-label'>כתובת:</td>
                        <td class='info-value'>{EncodeHtml(model.Offer.Customer.Address)}</td>
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
                        <td class='info-label'>טיול:</td>
                        <td class='info-value'>{EncodeHtml(model.Offer.Tour?.Title)}</td>
                    </tr>
                    <tr>
                        <td class='info-label'>תאריך:</td>
                        <td class='info-value'>{model.Offer.TourDate:dd/MM/yyyy}</td>
                    </tr>
                    <tr>
                        <td class='info-label'>משתתפים:</td>
                        <td class='info-value'>{model.Offer.Participants}</td>
                    </tr>");

            if (!string.IsNullOrEmpty(model.Offer.PickupLocation))
            {
                html.AppendLine($@"
                    <tr>
                        <td class='info-label'>נקודת איסוף:</td>
                        <td class='info-value'>{EncodeHtml(model.Offer.PickupLocation)}</td>
                    </tr>");
            }

            html.AppendLine(@"
                </table>
            </div>
        </div>");

            // Guide details
            html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>פרטי המדריך</div>
            <div class='section-content'>
                <table class='info-table'>
                    <tr>
                        <td class='info-label'>שם המדריך:</td>
                        <td class='info-value'>{EncodeHtml(model.Offer.Guide?.GuideName)}</td>
                    </tr>");

            if (!string.IsNullOrEmpty(model.Offer.Guide?.Description))
            {
                html.AppendLine($@"
                    <tr>
                        <td class='info-label'>תיאור:</td>
                        <td class='info-value'>{EncodeHtml(model.Offer.Guide.Description)}</td>
                    </tr>");
            }

            html.AppendLine(@"
                </table>
            </div>
        </div>");

            // Trip description
            if (!string.IsNullOrEmpty(model.Offer.Tour?.Description))
            {
                html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>אודות הסיור - {EncodeHtml(model.Offer.Tour.Title)}</div>
            <div class='section-content'>
                <p>{EncodeHtml(model.Offer.Tour.Description).Replace("\n", "<br/>")}</p>
            </div>
        </div>");
            }

            // Schedule
            if (model.Offer.Tour?.Schedule != null && model.Offer.Tour.Schedule.Any())
            {
                html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>לוח זמנים מפורט</div>
            <div class='section-content'>");

                foreach (var item in model.Offer.Tour.Schedule.OrderBy(s => s.StartTime))
                {
                    html.AppendLine($@"
                <div class='schedule-item'>
                    <div class='schedule-time'>{item.StartTime:hh\\:mm}");

                    if (item.EndTime.HasValue)
                    {
                        html.AppendLine($@" - {item.EndTime.Value:hh\\:mm}");
                    }

                    html.AppendLine($@"</div>
                    <div class='schedule-location'>{EncodeHtml(item.Location, "")}</div>
                    <div class='schedule-description'>{EncodeHtml(item.Description, "").Replace("\n", "<br/>")}</div>
                </div>");
                }

                html.AppendLine(@"
            </div>
        </div>");
            }

            // What tour includes
            if (model.Offer.Tour?.Includes != null && model.Offer.Tour.Includes.Any())
            {
                html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>הסיור כולל</div>
            <div class='section-content'>
                <ul class='check-list includes'>");

                foreach (var include in model.Offer.Tour.Includes)
                {
                    html.AppendLine($@"
                    <li>{EncodeHtml(include.Description ?? include.Text, "")}</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>
        </div>");
            }

            // What tour excludes
            if (model.Offer.Tour?.Excludes != null && model.Offer.Tour.Excludes.Any())
            {
                html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>הסיור לא כולל</div>
            <div class='section-content'>
                <ul class='check-list excludes'>");

                foreach (var exclude in model.Offer.Tour.Excludes)
                {
                    html.AppendLine($@"
                    <li>{EncodeHtml(exclude.Description ?? exclude.Text, "")}</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>
        </div>");
            }

            // Price includes
            if (!string.IsNullOrEmpty(model.Offer.PriceIncludes))
            {
                html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>המחיר כולל</div>
            <div class='section-content'>
                <ul class='check-list includes'>");

                var priceIncludes = model.Offer.PriceIncludes.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in priceIncludes)
                {
                    html.AppendLine($@"
                    <li>{EncodeHtml(item.Trim())}</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>
        </div>");
            }

            // Price excludes
            if (!string.IsNullOrEmpty(model.Offer.PriceExcludes))
            {
                html.AppendLine(@"
        <div class='section'>
            <div class='section-title'>המחיר אינו כולל</div>
            <div class='section-content'>
                <ul class='check-list excludes'>");

                var priceExcludes = model.Offer.PriceExcludes.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in priceExcludes)
                {
                    html.AppendLine($@"
                    <li>{EncodeHtml(item.Trim())}</li>");
                }

                html.AppendLine(@"
                </ul>
            </div>
        </div>");
            }

            // Special requests
            if (!string.IsNullOrEmpty(model.Offer.SpecialRequests))
            {
                html.AppendLine($@"
        <div class='section'>
            <div class='section-title'>בקשות מיוחדות וטקסים</div>
            <div class='section-content'>
                <p>{EncodeHtml(model.Offer.SpecialRequests).Replace("\n", "<br/>")}</p>
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
            <div class='section-title'>פרטי מחיר</div>
            <table class='info-table'>
                <tr>
                    <td class='info-label'>מחיר לאדם:</td>
                    <td class='info-value'><span class='price-total'>₪{model.Offer.Price:N0}</span></td>
                </tr>
                <tr>
                    <td class='info-label'>סה״כ לתשלום:</td>
                    <td class='info-value'><span class='price-total'>₪{model.Offer.TotalPayment:N0}</span></td>
                </tr>");

            if (model.PaymentMethod != null)
            {
                html.AppendLine($@"
                <tr>
                    <td class='info-label'>שיטת תשלום:</td>
                    <td class='info-value'>{EncodeHtml(model.PaymentMethod.METHOD)}</td>
                </tr>");
            }

            html.AppendLine($@"
                <tr>
                    <td class='info-label'>ארוחת צהריים:</td>
                    <td class='info-value'>{(model.Offer.LunchIncluded ? "כלולה במחיר" : "לא כלולה")}</td>
                </tr>
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
                        <td class='info-value'><strong style='color: #38a169;'>₪{model.Offer.TotalPayment:N0}</strong></td>
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
    }
}
