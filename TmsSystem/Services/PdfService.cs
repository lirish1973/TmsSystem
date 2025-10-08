using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.Kernel.Font;
using TmsSystem.ViewModels;
using System.Text;
using System.Linq;

namespace TmsSystem.Services
{
    public class PdfService : IPdfService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PdfService> _logger;

        public PdfService(IWebHostEnvironment environment, ILogger<PdfService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<byte[]> GenerateOfferPdfAsync(ShowOfferViewModel model)
        {
            try
            {
                _logger.LogInformation("Starting PDF generation for offer {OfferId}", model.Offer.OfferId);

                var htmlContent = await GenerateHtmlContent(model);
                _logger.LogInformation("HTML content generated successfully");

                using var memoryStream = new MemoryStream();
                using (var writer = new PdfWriter(memoryStream))
                {
                    writer.SetCloseStream(false);

                    using var pdfDoc = new PdfDocument(writer);
                    pdfDoc.SetDefaultPageSize(PageSize.A4);

                    var props = new ConverterProperties();

                    // הגדרת פונטים
                    var fontProvider = new DefaultFontProvider(true, true, true);

                    // נסה להוסיף פונטים תומכי עברית
                    await AddHebrewFonts(fontProvider);

                    props.SetFontProvider(fontProvider);
                    props.SetCharset("UTF-8");
                    props.SetBaseUri("");

                    // המרת HTML ל-PDF
                    HtmlConverter.ConvertToPdf(htmlContent, pdfDoc, props);

                    _logger.LogInformation("PDF conversion completed successfully");
                }

                var result = memoryStream.ToArray();
                _logger.LogInformation("PDF generated successfully. Size: {Size} bytes", result.Length);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for offer {OfferId}: {Message}",
                    model?.Offer?.OfferId, ex.Message);
                throw new InvalidOperationException($"שגיאה ביצירת PDF: {ex.Message}", ex);
            }
        }

        private async Task AddHebrewFonts(DefaultFontProvider fontProvider)
        {
            try
            {
                // נסה Windows fonts
                var windowsFonts = new[]
                {
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF"),
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf"),
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF")
                };

                foreach (var fontPath in windowsFonts)
                {
                    if (System.IO.File.Exists(fontPath))
                    {
                        fontProvider.AddFont(fontPath);
                        _logger.LogInformation("Added Windows font: {FontPath}", fontPath);
                        return;
                    }
                }

                // נסה Linux fonts
                var linuxFonts = new[]
                {
                    "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
                    "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
                    "/usr/share/fonts/TTF/DejaVuSans.ttf",
                    "/System/Library/Fonts/Arial.ttf"
                };

                foreach (var fontPath in linuxFonts)
                {
                    if (System.IO.File.Exists(fontPath))
                    {
                        fontProvider.AddFont(fontPath);
                        _logger.LogInformation("Added Linux/Mac font: {FontPath}", fontPath);
                        return;
                    }
                }

                _logger.LogWarning("No Hebrew-supporting fonts found, using default fonts");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error adding Hebrew fonts: {Message}", ex.Message);
            }
        }

        // פונקציה להיפוך טקסט עברי
        private string ReverseHebrew(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var chars = text.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        private async Task<string> GenerateHtmlContent(ShowOfferViewModel model)
        {
            try
            {
                var logoBase64 = await GetLogoBase64();

                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html dir='rtl' lang='he'>");
                html.AppendLine("<head>");
                html.AppendLine("    <meta charset='UTF-8'>");
                html.AppendLine("    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>");
                html.AppendLine($"    <title>הצעת מחיר #{model.Offer.OfferId}</title>");
                html.AppendLine("    <style>");
                html.AppendLine(GetCssStyles());
                html.AppendLine("    </style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                // Header
                html.AppendLine("    <div class='header'>");
                if (!string.IsNullOrEmpty(logoBase64))
                {
                    html.AppendLine($"        <img src='data:image/png;base64,{logoBase64}' class='logo' alt='לוגו'>");
                }
                html.AppendLine($"        <div class='company-name'>{ReverseHebrew("מערכת ניהול טיולים - TMS")}</div>");
                html.AppendLine($"        <h1>{ReverseHebrew($"הצעת מחיר #{model.Offer.OfferId}")}</h1>");
                html.AppendLine($"        <p>{ReverseHebrew($"נוצרה: {model.Offer.CreatedAt:dd/MM/yyyy HH:mm}")}</p>");
                html.AppendLine("    </div>");

                // פרטי הלקוח
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrew("פרטי הלקוח")}</div>");
                html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("שם מלא:")}</span> {ReverseHebrew(System.Web.HttpUtility.HtmlEncode(model.Offer.Customer.FullName ?? ""))}</div>");
                html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("טלפון:")}</span> {System.Web.HttpUtility.HtmlEncode(model.Offer.Customer.Phone ?? "")}</div>");
                html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("אימייל:")}</span> {System.Web.HttpUtility.HtmlEncode(model.Offer.Customer.Email ?? "")}</div>");
                html.AppendLine("    </div>");

                // פרטי הטיול
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrew("פרטי הטיול")}</div>");
                html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("טיול:")}</span> {ReverseHebrew(System.Web.HttpUtility.HtmlEncode(model.Offer.Tour.Title ?? ""))}</div>");
                html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("תאריך:")}</span> {model.Offer.TourDate:dd/MM/yyyy}</div>");
                html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("משתתפים:")}</span> {model.Offer.Participants}</div>");
                if (!string.IsNullOrEmpty(model.Offer.PickupLocation))
                {
                    html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("נקודת איסוף:")}</span> {ReverseHebrew(System.Web.HttpUtility.HtmlEncode(model.Offer.PickupLocation))}</div>");
                }
                html.AppendLine("    </div>");

                // פרטי המדריך
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrew("פרטי המדריך")}</div>");
                html.AppendLine($"        <div class='info-row'><span class='info-label'>{ReverseHebrew("מדריך:")}</span> {ReverseHebrew(System.Web.HttpUtility.HtmlEncode(model.Offer.Guide.GuideName ?? ""))}</div>");
                html.AppendLine("    </div>");

                // מחיר
                html.AppendLine("    <div class='price-box'>");
                html.AppendLine($"        <div>{ReverseHebrew("מחיר לאדם:")} <span class='price-highlight'>₪{model.Offer.Price:N2}</span></div>");
                html.AppendLine($"        <div>{ReverseHebrew("סה\"כ:")} <span class='price-highlight'>₪{model.Offer.TotalPayment:N2}</span></div>");
                html.AppendLine("    </div>");

                // Footer
                html.AppendLine("    <div class='footer'>");
                html.AppendLine($"        <p><strong>{ReverseHebrew("תודה שבחרתם בנו!")}</strong></p>");
                html.AppendLine($"        <p>{ReverseHebrew("הצעה תקפה ל-30 יום")}</p>");
                html.AppendLine("    </div>");

                html.AppendLine("</body>");
                html.AppendLine("</html>");

                return html.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating HTML content: {Message}", ex.Message);
                throw;
            }
        }

        private string GetCssStyles()
        {
            return @"
        * {
            font-family: 'Arial', sans-serif;
        }
        body { 
            font-family: 'Arial', sans-serif; 
            direction: ltr; 
            text-align: left;
            margin: 20px; 
            font-size: 14px;
            background: white;
        }
        .header { 
            text-align: center; 
            border-bottom: 3px solid #007bff; 
            padding-bottom: 20px; 
            margin-bottom: 30px; 
        }
        .logo { 
            max-width: 200px; 
            height: auto; 
            margin-bottom: 15px; 
        }
        .company-name { 
            font-size: 24px; 
            font-weight: bold; 
            color: #007bff; 
            margin: 10px 0; 
        }
        .section { 
            margin-bottom: 20px; 
            padding: 15px; 
            border: 1px solid #ddd; 
            border-radius: 5px; 
            background: #f8f9fa;
            text-align: left;
        }
        .section-title { 
            font-weight: bold; 
            color: #007bff; 
            margin-bottom: 10px; 
            text-align: right;
        }
        .info-row { 
            margin-bottom: 8px; 
            text-align: right;
        }
        .info-label { 
            font-weight: bold; 
            display: inline-block; 
            width: 120px; 
            text-align: right;
        }
        .price-box { 
            background: linear-gradient(135deg, #28a745, #20c997); 
            color: white;
            padding: 20px; 
            text-align: center; 
            border-radius: 10px; 
            font-size: 18px; 
            margin: 20px 0;
        }
        .price-highlight {
            font-size: 24px;
            font-weight: bold;
        }
        .footer {
            text-align: center; 
            margin-top: 40px; 
            border-top: 2px solid #007bff; 
            padding-top: 20px;
        }";
        }

        private async Task<string> GetLogoBase64()
        {
            try
            {
                var logoPath = System.IO.Path.Combine(_environment.WebRootPath, "images", "logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    var logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);
                    return Convert.ToBase64String(logoBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load logo: {Message}", ex.Message);
            }
            return string.Empty;
        }
    }
}