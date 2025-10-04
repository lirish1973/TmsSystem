using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using System.IO;
using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
   

    public class PdfService : IPdfService
    {
        private readonly IWebHostEnvironment _environment;

        public PdfService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<byte[]> GenerateOfferPdfAsync(ShowOfferViewModel model)
        {
            var htmlContent = await GenerateHtmlContent(model);

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(memoryStream))
                {
                    writer.SetCloseStream(false); // Keep the stream open after writer disposal
                    using (var pdfDoc = new PdfDocument(writer))
                    {
                        pdfDoc.SetDefaultPageSize(PageSize.A4);

                        // הגדרות תמיכה בעברית ו-RTL
                        var props = new ConverterProperties();
                        var fontProvider = new DefaultFontProvider(true, true, true);

                        // Try to add Hebrew-supporting fonts
                        try
                        {
                            // Try Windows fonts first
                            var windowsFontPath = System.IO.Path.Combine(
                                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts),
                                "ARIAL.TTF"
                            );
                            
                            if (System.IO.File.Exists(windowsFontPath))
                            {
                                fontProvider.AddFont(windowsFontPath);
                            }
                            else
                            {
                                // Try Linux fonts
                                var linuxFontPaths = new[]
                                {
                                    "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
                                    "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf"
                                };

                                foreach (var fontPath in linuxFontPaths)
                                {
                                    if (System.IO.File.Exists(fontPath))
                                    {
                                        fontProvider.AddFont(fontPath);
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // If font loading fails, continue with default fonts
                        }

                        props.SetFontProvider(fontProvider);
                        props.SetCharset("UTF-8");

                        // המרת HTML ל-PDF
                        HtmlConverter.ConvertToPdf(htmlContent, pdfDoc, props);
                    }
                }

                return memoryStream.ToArray();
            }
        }


        private async Task<string> GenerateHtmlContent(ShowOfferViewModel model)
        {
            var logoBase64 = await GetLogoBase64();

            return $@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <title>הצעת מחיר #{model.Offer.OfferId}</title>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            direction: rtl; 
            margin: 20px; 
            font-size: 14px;
            background: white;
        }}
        .header {{ 
            text-align: center; 
            border-bottom: 3px solid #007bff; 
            padding-bottom: 20px; 
            margin-bottom: 30px; 
        }}
        .logo {{ 
            max-width: 200px; 
            height: auto; 
            margin-bottom: 15px; 
        }}
        .company-name {{ 
            font-size: 24px; 
            font-weight: bold; 
            color: #007bff; 
            margin: 10px 0; 
        }}
        .section {{ 
            margin-bottom: 20px; 
            padding: 15px; 
            border: 1px solid #ddd; 
            border-radius: 5px; 
            background: #f8f9fa;
        }}
        .section-title {{ 
            font-weight: bold; 
            color: #007bff; 
            margin-bottom: 10px; 
        }}
        .info-row {{ 
            margin-bottom: 8px; 
        }}
        .info-label {{ 
            font-weight: bold; 
            display: inline-block; 
            width: 120px; 
        }}
        .price-box {{ 
            background: linear-gradient(135deg, #28a745, #20c997); 
            color: white;
            padding: 20px; 
            text-align: center; 
            border-radius: 10px; 
            font-size: 18px; 
        }}
        .price-highlight {{
            font-size: 24px;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='header'>
        {(!string.IsNullOrEmpty(logoBase64) ? $"<img src='data:image/png;base64,{logoBase64}' class='logo' alt='לוגו'>" : "")}
        <div class='company-name'>מערכת ניהול טיולים - TMS</div>
        <h1>הצעת מחיר #{model.Offer.OfferId}</h1>
        <p>נוצרה: {model.Offer.CreatedAt:dd/MM/yyyy HH:mm}</p>
    </div>
    
    <div class='section'>
        <div class='section-title'>פרטי הלקוח</div>
        <div class='info-row'>
            <span class='info-label'>שם מלא:</span>
            {model.Offer.Customer.FullName}
        </div>
        <div class='info-row'>
            <span class='info-label'>טלפון:</span>
            {model.Offer.Customer.Phone}
        </div>
        <div class='info-row'>
            <span class='info-label'>אימייל:</span>
            {model.Offer.Customer.Email}
        </div>
    </div>
    
    <div class='section'>
        <div class='section-title'>פרטי הטיול</div>
        <div class='info-row'>
            <span class='info-label'>טיול:</span>
            {model.Offer.Tour.Title}
        </div>
        <div class='info-row'>
            <span class='info-label'>תאריך:</span>
            {model.Offer.TourDate:dd/MM/yyyy}
        </div>
        <div class='info-row'>
            <span class='info-label'>משתתפים:</span>
            {model.Offer.Participants}
        </div>
    </div>
    
    <div class='section'>
        <div class='section-title'>פרטי המדריך</div>
        <div class='info-row'>
            <span class='info-label'>מדריך:</span>
            {model.Offer.Guide.GuideName}
        </div>
    </div>
    
    <div class='price-box'>
        <div>מחיר לאדם: <span class='price-highlight'>₪{model.Offer.Price:N2}</span></div>
        <div>סה""כ: <span class='price-highlight'>₪{model.Offer.TotalPayment:N2}</span></div>
    </div>
    
    <div style='text-align: center; margin-top: 40px; border-top: 2px solid #007bff; padding-top: 20px;'>
        <p><strong>תודה שבחרתם בנו!</strong></p>
        <p>הצעה תקפה ל-30 יום</p>
    </div>
</body>
</html>";
        }

        private async Task<string> GetLogoBase64()
        {
            try
            {
                // ✅ שימוש בשם מלא כדי למנוע התנגשות עם iText.Kernel.Geom.Path
                var logoPath = System.IO.Path.Combine(_environment.WebRootPath, "images", "logo.png");

                if (System.IO.File.Exists(logoPath))
                {
                    var logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);
                    return Convert.ToBase64String(logoBytes);
                }
            }
            catch
            {
                // אם יש שגיאה, החזר ריק
            }

            return string.Empty;
        }

    }
}