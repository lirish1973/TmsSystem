using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Linq;
using System.Text;
using System.Globalization;
using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
    public partial class PdfService : IPdfService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PdfService> _logger;

        public PdfService(IWebHostEnvironment environment, ILogger<PdfService> logger)
        {
            _environment = environment;
            _logger = logger;
        }


       // public Task<string> GenerateOfferHtmlAsync(ShowOfferViewModel model)
      //  {
            // המתודה GenerateHtmlContent קיימת כבר בקובץ המקורי (private) ונגישה פה כי זו אותה מחלקה (partial)
        //    return GenerateHtmlContent(model);
     //   }


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




        // פונקציה משופרת - הופכת תווים בכל מילה עברית והופכת סדר מילים
        private string ReverseHebrewSafely(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            bool hasHebrew = text.Any(c => c >= '\u0590' && c <= '\u05FF');
            if (!hasHebrew)
                return text;

            // Step 1: פיצול למילים (כולל שמירה על רווחים ופיסוק)
            var parts = text.Split(' ');
            var processedParts = new List<string>();

            foreach (var part in parts)
            {
                bool partHasHebrew = part.Any(c => c >= '\u0590' && c <= '\u05FF');

                if (partHasHebrew)
                {
                    // הפוך רק את התווים במילה
                    var chars = part.ToCharArray();
                    Array.Reverse(chars);
                    processedParts.Add(new string(chars));
                }
                else
                {
                    // השאר אנגלית, מספרים וסימנים
                    processedParts.Add(part);
                }
            }

            // Step 2: הפוך את סדר המילים
            processedParts.Reverse();

            return string.Join(" ", processedParts);
        }



        // פונקציה חדשה - רק הופכת תווים, לא מילים
        private string ReverseHebrewCharsOnly(string text)
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
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("    <meta charset='UTF-8'>");
                html.AppendLine("    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>");
                html.AppendLine($"    <title>{ReverseHebrewSafely("הצעת מחיר")} #{model.Offer.OfferId}</title>");
                html.AppendLine("    <style>");
                html.AppendLine(GetCssStyles());
                html.AppendLine("    </style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");

                // Header
                html.AppendLine("    <div class='header'>");
                if (!string.IsNullOrEmpty(logoBase64))
                {
                    html.AppendLine($"        <img src='data:image/png;base64,{logoBase64}' class='logo' alt='logo'>");
                }
                html.AppendLine($"        <div class='company-name'>{ReverseHebrewSafely("מערכת ניהול טיולים")} - TMS</div>");
                html.AppendLine($"        <h1>{ReverseHebrewSafely("הצעת מחיר")} #{model.Offer.OfferId}</h1>");
                html.AppendLine($"        <p>{ReverseHebrewSafely("נוצרה:")} {model.Offer.CreatedAt:dd/MM/yyyy HH:mm}</p>");
                html.AppendLine("    </div>");

                // פרטי הלקוח
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("פרטי הלקוח")}</div>");
                html.AppendLine($"        <div class='info-row'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(model.Offer.Customer.FullName ?? ""))} <span class='info-label'>{ReverseHebrewSafely(":שם מלא")}</span></div>");
                html.AppendLine($"        <div class='info-row'>{System.Web.HttpUtility.HtmlEncode(model.Offer.Customer.Phone ?? "")} <span class='info-label'>{ReverseHebrewSafely(":טלפון")}</span></div>");
                html.AppendLine($"        <div class='info-row'>{System.Web.HttpUtility.HtmlEncode(model.Offer.Customer.Email ?? "")} <span class='info-label'>{ReverseHebrewSafely(":אימייל")}</span></div>");
                html.AppendLine("    </div>");

                // פרטי הטיול
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("פרטי הטיול")}</div>");
                html.AppendLine($"        <div class='info-row'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(model.Offer.Tour.Title ?? ""))} <span class='info-label'>{ReverseHebrewSafely(":טיול")}</span></div>");
                html.AppendLine($"        <div class='info-row'>{model.Offer.TourDate:dd/MM/yyyy} <span class='info-label'>{ReverseHebrewSafely(":תאריך")}</span></div>");
                html.AppendLine($"        <div class='info-row'>{model.Offer.Participants} <span class='info-label'>{ReverseHebrewSafely(":משתתפים")}</span></div>");
                if (!string.IsNullOrEmpty(model.Offer.PickupLocation))
                {
                    html.AppendLine($"        <div class='info-row'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(model.Offer.PickupLocation))} <span class='info-label'>{ReverseHebrewSafely(":נקודת איסוף")}</span></div>");
                }
                html.AppendLine("    </div>");

                // פרטי המדריך
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("פרטי המדריך")}</div>");
                html.AppendLine($"        <div class='info-row'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(model.Offer.Guide.GuideName ?? ""))} <span class='info-label'>{ReverseHebrewSafely(":מדריך")}</span></div>");
                html.AppendLine("    </div>");




                // ✅ תיאור הסיור - TEST WITHOUT REVERSE (סדר שורות מקורי)
                if (!string.IsNullOrEmpty(model.Offer.Tour?.Description))
                {
                    html.AppendLine("    <div class='section'>");
                    html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("אודות הסיור")} - {ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(model.Offer.Tour.Title))}</div>");

                    var description = System.Web.HttpUtility.HtmlEncode(model.Offer.Tour.Description);

                    var lines = description.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    // אם בעבר שמת כאן Array.Reverse(lines); אל תחזיר אותו אם אתה רוצה את הסדר המקורי

                    var processedLines = lines.Select(line =>
                    {
                        var trimmed = line.Replace("\r", "").Trim();
                        if (string.IsNullOrWhiteSpace(trimmed))
                            return "";

                        // ===== אם האותיות יוצאות הפוך (ולא קריא) הסר את הסלאשים מהקטע הבא =====
                        
                        var chars = trimmed.ToCharArray();
                        Array.Reverse(chars);
                        trimmed = new string(chars);
                        
                        // ===== סוף קטע היפוך אותיות =====

                        return trimmed;
                    });

                    html.AppendLine("        <div class='tour-description' style='direction:rtl; text-align:right; font-family: Arial;'>");
                    html.AppendLine($"            {string.Join("<br />", processedLines.Where(l => !string.IsNullOrEmpty(l)))}");
                    html.AppendLine("        </div>");
                    html.AppendLine("    </div>");
                }









                // ✅ לוח זמנים מפורט - SECTION 2
                if (model.Offer.Tour?.Schedule != null && model.Offer.Tour.Schedule.Any())
                {
                    html.AppendLine("    <div class='section'>");
                    html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("לוח זמנים מפורט")}</div>");
                    html.AppendLine("        <div class='schedule-timeline'>");

                    foreach (var scheduleItem in model.Offer.Tour.Schedule.OrderBy(s => s.StartTime))
                    {
                        html.AppendLine("            <div class='schedule-item'>");
                        html.AppendLine("                <div class='schedule-time'>");

                        // Format time properly
                        string timeDisplay = string.Format("{0:D2}:{1:D2}",
                            scheduleItem.StartTime.Hours,
                            scheduleItem.StartTime.Minutes);

                        if (scheduleItem.EndTime.HasValue && scheduleItem.EndTime.Value != TimeSpan.Zero)
                        {
                            timeDisplay += string.Format(" - {0:D2}:{1:D2}",
                                scheduleItem.EndTime.Value.Hours,
                                scheduleItem.EndTime.Value.Minutes);
                        }

                        html.AppendLine($"                    <span class='time-badge'>{timeDisplay}</span>");
                        html.AppendLine("                </div>");
                        html.AppendLine("                <div class='schedule-content'>");
                        html.AppendLine($"                    <h4 class='location-title'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(scheduleItem.Location ?? ""))}</h4>");
                        if (!string.IsNullOrEmpty(scheduleItem.Description))
                        {
                            html.AppendLine($"                    <p>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(scheduleItem.Description))}</p>");
                        }
                        html.AppendLine("                </div>");
                        html.AppendLine("            </div>");
                    }

                    html.AppendLine("        </div>");
                    html.AppendLine("    </div>");
                }

                // ✅ הסיור כולל / לא כולל - SECTION 3
                if ((model.Offer.Tour?.Includes != null && model.Offer.Tour.Includes.Any()) ||
                    (model.Offer.Tour?.Excludes != null && model.Offer.Tour.Excludes.Any()))
                {
                    html.AppendLine("    <div class='section'>");
                    html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("הסיור כולל לא כולל /")}</div>");
                    html.AppendLine("        <div class='row'>");

                    // הסיור כולל
                    if (model.Offer.Tour?.Includes != null && model.Offer.Tour.Includes.Any())
                    {
                        html.AppendLine("            <div class='col-md-6'>");
                        html.AppendLine($"                <h5 class='includes-title'>&#10003; {ReverseHebrewSafely("הסיור כולל")}</h5>");
                        html.AppendLine("                <ul class='includes-list'>");
                        foreach (var include in model.Offer.Tour.Includes)
                        {
                            html.AppendLine($"                    <li class='include-item-tour'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(include.Description ?? ""))}</li>");
                        }
                        html.AppendLine("                </ul>");
                        html.AppendLine("            </div>");
                    }

                    // הסיור לא כולל
                    if (model.Offer.Tour?.Excludes != null && model.Offer.Tour.Excludes.Any())
                    {
                        html.AppendLine("            <div class='col-md-6'>");
                        html.AppendLine($"                <h5 class='excludes-title'>&#10007; {ReverseHebrewSafely("הסיור לא כולל")}</h5>");
                        html.AppendLine("                <ul class='excludes-list'>");
                        foreach (var exclude in model.Offer.Tour.Excludes)
                        {
                            html.AppendLine($"                    <li class='exclude-item-tour'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(exclude.Description ?? ""))}</li>");
                        }
                        html.AppendLine("                </ul>");
                        html.AppendLine("            </div>");
                    }

                    html.AppendLine("        </div>");
                    html.AppendLine("    </div>");
                }

                // מחיר
                html.AppendLine("    <div class='price-box'>");
                html.AppendLine($"        <div>{ReverseHebrewSafely("מחיר לאדם:")} <span class='price-highlight'>&#8362;{model.Offer.Price:N2}</span></div>");
                html.AppendLine($"        <div>{ReverseHebrewSafely("סה\"כ:")} <span class='price-highlight'>&#8362;{model.Offer.TotalPayment:N2}</span></div>");
                html.AppendLine("    </div>");

                // ✅ המחיר כולל - SECTION 4
                if (!string.IsNullOrEmpty(model.Offer.PriceIncludes))
                {
                    html.AppendLine("    <div class='section'>");
                    html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("המחיר כולל")}</div>");
                    html.AppendLine("        <ul class='price-includes-list'>");
                    var priceIncludes = model.Offer.PriceIncludes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in priceIncludes)
                    {
                        html.AppendLine($"            <li class='price-include-item'>&#10003; {ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(item.Trim()))}</li>");
                    }
                    html.AppendLine("        </ul>");
                    html.AppendLine("    </div>");
                }

                // ✅ המחיר אינו כולל - SECTION 5
                if (!string.IsNullOrEmpty(model.Offer.PriceExcludes))
                {
                    html.AppendLine("    <div class='section'>");
                    html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("המחיר אינו כולל")}</div>");
                    html.AppendLine("        <ul class='price-excludes-list'>");
                    var priceExcludes = model.Offer.PriceExcludes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in priceExcludes)
                    {
                        html.AppendLine($"            <li class='price-exclude-item'>&#10007; {ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(item.Trim()))}</li>");
                    }
                    html.AppendLine("        </ul>");
                    html.AppendLine("    </div>");
                }

                // ✅ בקשות מיוחדות - SECTION 6
                if (!string.IsNullOrEmpty(model.Offer.SpecialRequests))
                {
                    html.AppendLine("    <div class='section'>");
                    html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("בקשות מיוחדות וטקסים")}</div>");

                    var specialRequests = System.Web.HttpUtility.HtmlEncode(model.Offer.SpecialRequests);

                    html.AppendLine("        <div class='special-requests'>");

                    // אל תפצל לשורות - פשוט הצג הכל ביחד
                    html.AppendLine($"            {ReverseHebrewSafely(specialRequests.Replace("\n", "<br />").Replace("\r", ""))}");

                    html.AppendLine("        </div>");
                    html.AppendLine("    </div>");
                }

                // ✅ תנאי ביטול ותשלום - SECTION 7
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("תנאי ביטול ותשלום")}</div>");
                html.AppendLine("        <ul class='terms-list'>");
                html.AppendLine($"            <li class='term-item'><strong>{ReverseHebrewSafely("תשלום מקדמה:")}</strong> {ReverseHebrewSafely("נדרש תשלום מקדמה של 30% מהמחיר הכולל לאישור ההזמנה.")}</li>");
                html.AppendLine($"            <li class='term-item'><strong>{ReverseHebrewSafely("דחיית תאריך:")}</strong> {ReverseHebrewSafely("ניתן לדחות את התאריך עד 14 יום לפני מועד הטיול ללא עלות.")}</li>");
                html.AppendLine($"            <li class='term-item'><strong>{ReverseHebrewSafely("ביטול עד 30 יום:")}</strong> {ReverseHebrewSafely("ביטול עד 30 יום לפני התאריך החזר מלא בניכוי עמלת ביטול של 5%.")}</li>");
                html.AppendLine($"            <li class='term-item'><strong>{ReverseHebrewSafely("ביטול 30-14 ימים:")}</strong> {ReverseHebrewSafely("ביטול בין 30 ל-14 יום לפני התאריך החזר של 50% מהתשלום.")}</li>");
                html.AppendLine($"            <li class='term-item'><strong>{ReverseHebrewSafely("ביטול עד 14 ימים:")}</strong> {ReverseHebrewSafely("ביטול פחות מ-14 יום לפני התאריך אין החזר כספי.")}</li>");
                html.AppendLine($"            <li class='term-item'><strong>{ReverseHebrewSafely("ביטול מטעמנו:")}</strong> {ReverseHebrewSafely("במקרה של ביטול מטעמנו יינתן החזר מלא.")}</li>");
                html.AppendLine("        </ul>");
                html.AppendLine("    </div>");

                // ✅ פרטי תשלום והעברה בנקאית - SECTION 8
                html.AppendLine("    <div class='section'>");
                html.AppendLine($"        <div class='section-title'>{ReverseHebrewSafely("פרטי תשלום")}</div>");
                if (model.PaymentMethod != null && !string.IsNullOrEmpty(model.PaymentMethod.METHOD))
                {
                    html.AppendLine($"        <div class='info-row'>{ReverseHebrewSafely(System.Web.HttpUtility.HtmlEncode(model.PaymentMethod.METHOD))} <span class='info-label'>{ReverseHebrewSafely(":שיטת תשלום")}</span></div>");
                }
                html.AppendLine($"        <div class='info-row'>&#8362;{model.Offer.TotalPayment:N2} <span class='info-label'>{ReverseHebrewSafely(":סכום לתשלום")}</span></div>");

                html.AppendLine($"        <h4 class='bank-title'>{ReverseHebrewSafely("פרטי העברה בנקאית")}</h4>");
                html.AppendLine("        <div class='bank-details-box'>");
                html.AppendLine($"            <div class='bank-detail'>{ReverseHebrewSafely("בנק לאומי")} <strong>{ReverseHebrewSafely(":שם הבנק")}</strong></div>");
                html.AppendLine($"            <div class='bank-detail'>805 <strong>{ReverseHebrewSafely(":מספר סניף")}</strong></div>");
                html.AppendLine($"            <div class='bank-detail'>39820047 <strong>{ReverseHebrewSafely(":מספר חשבון")}</strong></div>");
                html.AppendLine($"            <div class='bank-detail'>{ReverseHebrewSafely("ספארי אפריקה בע\"מ")} <strong>{ReverseHebrewSafely(":שם בעל החשבון")}</strong></div>");
                html.AppendLine($"            <div class='bank-detail'>515323970 <strong>{ReverseHebrewSafely(":ח.פ")}</strong></div>");
                html.AppendLine($"            <div class='bank-detail highlight'>&#8362;{model.Offer.TotalPayment:N2} <strong>{ReverseHebrewSafely(":סכום להעברה")}</strong></div>");
                html.AppendLine("        </div>");

                html.AppendLine($"        <h4 class='contact-title'>{ReverseHebrewSafely("פרטי קשר")}</h4>");
                html.AppendLine("        <div class='contact-details'>");
                html.AppendLine($"            <div class='contact-item'>058-7818560 {ReverseHebrewSafely(":טלפון")} &#128222;</div>");
                html.AppendLine($"            <div class='contact-item'>info@tryit.co.il {ReverseHebrewSafely(":אימייל")} &#128231;</div>");
                html.AppendLine("        </div>");
                html.AppendLine("    </div>");

                // Footer
                html.AppendLine("    <div class='footer'>");
                html.AppendLine($"        <p><strong>{ReverseHebrewSafely("תודה שבחרתם בנו!")}</strong></p>");
                html.AppendLine($"        <p>{ReverseHebrewSafely("הצעה תקפה ל-30 יום")}</p>");
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
            background: #f8f9fa;
            border: 1px solid #ddd;
            border-radius: 5px;
            text-align: right;
        }
        .section-title { 
            font-weight: bold; 
            color: #007bff; 
            margin-bottom: 10px; 
            font-size: 18px;
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
        .tour-description {
            line-height: 1.6;
            padding: 10px 0;
            text-align: right;
        }
        .schedule-timeline {
            margin-top: 15px;
        }
        .schedule-item {
            margin-bottom: 20px;
            padding: 15px;
            background: white;
            border-right: 4px solid #007bff;
            border-radius: 5px;
            text-align: right;
        }
        .schedule-time {
            margin-bottom: 8px;
            text-align: right;
        }
        .time-badge {
            background: #007bff;
            color: white;
            padding: 5px 15px;
            border-radius: 20px;
            font-weight: bold;
            font-size: 14px;
        }
        .location-title {
            color: #007bff;
            margin: 10px 0 5px 0;
            font-weight: bold;
            font-size: 16px;
            text-align: right;
        }
        .schedule-content p {
            margin: 5px 0;
            line-height: 1.5;
            text-align: right;
        }
        .row {
            display: table;
            width: 100%;
        }
        .col-md-6 {
            display: table-cell;
            width: 50%;
            padding: 10px;
            vertical-align: top;
        }
        .includes-title {
            color: #28a745;
            font-size: 16px;
            font-weight: bold;
            margin-bottom: 10px;
            text-align: right;
        }
        .excludes-title {
            color: #dc3545;
            font-size: 16px;
            font-weight: bold;
            margin-bottom: 10px;
            text-align: right;
        }
        .includes-list, .excludes-list {
            list-style: none;
            padding: 0;
            text-align: right;
        }
        .include-item-tour {
            padding: 8px;
            margin-bottom: 5px;
            background: #d4edda;
            border-right: 3px solid #28a745;
            border-radius: 3px;
            text-align: right;
        }
        .exclude-item-tour {
            padding: 8px;
            margin-bottom: 5px;
            background: #f8d7da;
            border-right: 3px solid #dc3545;
            border-radius: 3px;
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
        .price-includes-list, .price-excludes-list {
            list-style: none;
            padding: 0;
            text-align: right;
        }
        .price-include-item {
            padding: 8px;
            margin-bottom: 5px;
            background: #e7f3ff;
            border-right: 3px solid #007bff;
            border-radius: 3px;
            text-align: right;
        }
        .price-exclude-item {
            padding: 8px;
            margin-bottom: 5px;
            background: #fff3cd;
            border-right: 3px solid #ffc107;
            border-radius: 3px;
            text-align: right;
        }
        .special-requests {
            line-height: 1.6;
            padding: 10px;
            background: white;
            border-radius: 5px;
            text-align: right;
        }
        .terms-list {
            list-style: none;
            padding: 0;
            text-align: right;
        }
        .term-item {
            padding: 10px;
            margin-bottom: 8px;
            background: white;
            border-right: 3px solid #007bff;
            border-radius: 3px;
            line-height: 1.5;
            text-align: right;
        }
        .bank-title, .contact-title {
            color: #007bff;
            margin-top: 20px;
            margin-bottom: 10px;
            font-size: 16px;
            font-weight: bold;
            text-align: right;
        }
        .bank-details-box {
            background: #e7f3ff;
            padding: 15px;
            border-radius: 5px;
            border: 1px solid #007bff;
            text-align: right;
        }
        .bank-detail {
            padding: 5px 0;
            margin-bottom: 5px;
            text-align: right;
        }
        .bank-detail.highlight {
            font-size: 18px;
            color: #007bff;
            margin-top: 10px;
            padding-top: 10px;
            border-top: 2px solid #007bff;
        }
        .contact-details {
            background: white;
            padding: 15px;
            border-radius: 5px;
            border: 1px solid #ddd;
            text-align: right;
        }
        .contact-item {
            padding: 5px 0;
            margin-bottom: 5px;
            font-size: 16px;
            text-align: right;
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