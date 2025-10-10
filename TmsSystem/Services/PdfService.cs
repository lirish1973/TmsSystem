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

            // HTML Header with improved styling
            html.AppendLine(@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>הצעת מחיר - TMS System</title>
    <style>
        body {
            font-family: 'Arial', 'Tahoma', sans-serif;
            line-height: 1.6;
            color: #333;
            background: #f8f9fa;
            margin: 0;
            padding: 20px;
            direction: rtl;
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
            background: linear-gradient(135deg, #2c5aa0 0%, #1a365d 100%);
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
        }
        .section {
            margin-bottom: 25px;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 8px;
            border-right: 4px solid #2c5aa0;
        }
        .section-title {
            font-size: 1.4em;
            font-weight: bold;
            color: #2c5aa0;
            margin-bottom: 15px;
            display: flex;
            align-items: center;
        }
        .section-title::before {
            content: '●';
            margin-left: 10px;
            color: #2c5aa0;
        }
        .info-row {
            margin-bottom: 12px;
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .info-label {
            font-weight: bold;
            color: #495057;
            min-width: 120px;
        }
        .info-value {
            color: #333;
            flex: 1;
            text-align: right;
        }
        .price-section {
            background: linear-gradient(135deg, #e8f4f8 0%, #d1ecf1 100%);
            border-right: 4px solid #17a2b8;
        }
        .price-highlight {
            font-size: 1.3em;
            font-weight: bold;
            color: #17a2b8;
        }
        .includes-excludes {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-top: 20px;
        }
        .includes, .excludes {
            padding: 20px;
            border-radius: 8px;
        }
        .includes {
            background: #d4edda;
            border-right: 4px solid #28a745;
        }
        .excludes {
            background: #f8d7da;
            border-right: 4px solid #dc3545;
        }
        .includes h4, .excludes h4 {
            margin-top: 0;
            color: #333;
        }
        .includes h4::before {
            content: '✓ ';
            color: #28a745;
        }
        .excludes h4::before {
            content: '✗ ';
            color: #dc3545;
        }
        ul {
            margin: 0;
            padding-right: 20px;
        }
        li {
            margin-bottom: 8px;
            line-height: 1.4;
        }
        .itinerary-section {
            background: #fff3cd;
            border-right: 4px solid #ffc107;
        }
        .schedule-item {
            background: white;
            padding: 15px;
            margin-bottom: 10px;
            border-radius: 6px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .schedule-time {
            font-weight: bold;
            color: #2c5aa0;
            font-size: 1.1em;
        }
        .schedule-location {
            font-weight: bold;
            color: #495057;
            margin: 5px 0;
        }
        .schedule-description {
            color: #666;
            line-height: 1.5;
        }
        .footer {
            background: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #dee2e6;
            color: #6c757d;
            font-size: 0.9em;
        }
        @media print {
            body { background: white; }
            .container { box-shadow: none; }
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>הצעת מחיר</h1>
            <p>מספר הצעה: #" + model.Offer.OfferId + @"</p>
            <p>תאריך יצירה: " + model.Offer.CreatedAt.ToString("dd/MM/yyyy HH:mm") + @"</p>
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
                    <span class='info-label'>שם הטיול:</span>
                    <span class='info-value'>" + HttpUtility.HtmlEncode(model.Offer.Tour?.Title ?? "לא צוין") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>תאריך הטיול:</span>
                    <span class='info-value'>" + model.Offer.TourDate.ToString("dd/MM/yyyy") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>מספר משתתפים:</span>
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
                <div class='section-title'>תיאור הטיול</div>
                <div style='line-height: 1.6; color: #333;'>" +
                    HttpUtility.HtmlEncode(model.Offer.Tour.Description).Replace("\n", "<br>") + @"
                </div>
            </div>");
            }

            // מחירים
            html.AppendLine(@"
            <div class='section price-section'>
                <div class='section-title'>פרטי מחיר</div>
                <div class='info-row'>
                    <span class='info-label'>מחיר לאדם:</span>
                    <span class='info-value price-highlight'>₪" + model.Offer.Price.ToString("N2") + @"</span>
                </div>
                <div class='info-row'>
                    <span class='info-label'>סה״כ לתשלום:</span>
                    <span class='info-value price-highlight'>₪" + model.Offer.TotalPayment.ToString("N2") + @"</span>
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
                </div>
            </div>");

            // כולל/לא כולל
            html.AppendLine(@"
            <div class='includes-excludes'>
                <div class='includes'>
                    <h4>כלול במחיר</h4>
                    <ul>");

            if (!string.IsNullOrEmpty(model.Offer.PriceIncludes))
            {
                var includes = model.Offer.PriceIncludes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includes)
                {
                    html.AppendLine("                        <li>" + HttpUtility.HtmlEncode(include.Trim()) + "</li>");
                }
            }
            else
            {
                html.AppendLine("                        <li>לא צוינו פרטים</li>");
            }

            html.AppendLine(@"
                    </ul>
                </div>
                <div class='excludes'>
                    <h4>לא כלול במחיר</h4>
                    <ul>");

            if (!string.IsNullOrEmpty(model.Offer.PriceExcludes))
            {
                var excludes = model.Offer.PriceExcludes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var exclude in excludes)
                {
                    html.AppendLine("                        <li>" + HttpUtility.HtmlEncode(exclude.Trim()) + "</li>");
                }
            }
            else
            {
                html.AppendLine("                        <li>לא צוינו פרטים</li>");
            }

            html.AppendLine(@"
                    </ul>
                </div>
            </div>");

            // בקשות מיוחדות
            if (!string.IsNullOrEmpty(model.Offer.SpecialRequests))
            {
                html.AppendLine(@"
            <div class='section'>
                <div class='section-title'>בקשות מיוחדות</div>
                <div style='line-height: 1.6; color: #333;'>" +
                    HttpUtility.HtmlEncode(model.Offer.SpecialRequests).Replace("\n", "<br>") + @"
                </div>
            </div>");
            }

            // Footer
            html.AppendLine(@"
        </div>
        <div class='footer'>
            <p>הצעה זו תקפה למשך 30 יום מתאריך ההנפקה</p>
            <p>מערכת ניהול טיולים - TMS System</p>
        </div>
    </div>
</body>
</html>");

            return html.ToString();
        }
    }
}