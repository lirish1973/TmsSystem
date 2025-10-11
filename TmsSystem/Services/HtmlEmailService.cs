using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
    /// <summary>
    /// שירות מתקדם לשליחת מיילי HTML עם תמונות מוטמעות באמצעות MimeKit
    /// </summary>
    public class HtmlEmailService : IHtmlEmailService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly SmtpOptions _smtpOptions;
        private readonly ILogger<HtmlEmailService> _logger;

        public HtmlEmailService(
            IWebHostEnvironment environment,
            IOptions<SmtpOptions> smtpOptions,
            ILogger<HtmlEmailService> logger)
        {
            _environment = environment;
            _smtpOptions = smtpOptions.Value;
            _logger = logger;
        }

        public async Task<string> GenerateOfferEmailHtmlAsync(ShowOfferViewModel model)
        {
            var html = new StringBuilder();

            // HTML עם עיצוב מותאם למיילים
            html.AppendLine(@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>הצעת מחיר - TMS System</title>
    <style>
        /* Reset for email clients */
        body, table, td, p, a, li {
            -webkit-text-size-adjust: 100%;
            -ms-text-size-adjust: 100%;
        }
        table, td {
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
        }
        img {
            -ms-interpolation-mode: bicubic;
            border: 0;
            height: auto;
            line-height: 100%;
            outline: none;
            text-decoration: none;
        }
        
        body {
            font-family: Arial, Tahoma, sans-serif;
            margin: 0;
            padding: 0;
            width: 100%;
            direction: rtl;
        }
        
        .email-container {
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
        }
        
        .header {
            background: linear-gradient(135deg, #2c5aa0 0%, #1a365d 100%);
            color: #ffffff;
            text-align: center;
            padding: 30px 20px;
        }
        
        .logo {
            max-width: 180px;
            height: auto;
            margin-bottom: 15px;
        }
        
        .header h1 {
            margin: 10px 0;
            font-size: 28px;
            font-weight: bold;
        }
        
        .header p {
            margin: 5px 0;
            font-size: 16px;
            opacity: 0.95;
        }
        
        .content {
            padding: 20px;
        }
        
        .section {
            margin-bottom: 20px;
            padding: 15px;
            background-color: #f8f9fa;
            border-right: 4px solid #2c5aa0;
            border-radius: 5px;
        }
        
        .section-title {
            font-size: 20px;
            font-weight: bold;
            color: #2c5aa0;
            margin-bottom: 12px;
            padding-bottom: 8px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .info-row {
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
        }
        
        .info-row:last-child {
            border-bottom: none;
        }
        
        .info-label {
            font-weight: bold;
            color: #495057;
            display: inline-block;
            min-width: 120px;
        }
        
        .info-value {
            color: #333333;
        }
        
        .price-section {
            background: linear-gradient(135deg, #e8f4f8 0%, #d1ecf1 100%);
            border-right-color: #17a2b8;
        }
        
        .price-highlight {
            font-size: 22px;
            font-weight: bold;
            color: #17a2b8;
        }
        
        .includes-excludes {
            margin-top: 15px;
        }
        
        .includes, .excludes {
            padding: 15px;
            margin-bottom: 15px;
            border-radius: 5px;
        }
        
        .includes {
            background-color: #d4edda;
            border-right: 4px solid #28a745;
        }
        
        .excludes {
            background-color: #f8d7da;
            border-right: 4px solid #dc3545;
        }
        
        .includes h4, .excludes h4 {
            margin: 0 0 10px 0;
            color: #333333;
            font-size: 16px;
        }
        
        ul {
            margin: 0;
            padding-right: 20px;
        }
        
        li {
            margin-bottom: 6px;
            line-height: 1.5;
        }
        
        .footer {
            background-color: #f8f9fa;
            color: #6c757d;
            text-align: center;
            padding: 20px;
            border-top: 1px solid #dee2e6;
            font-size: 14px;
        }
        
        .footer p {
            margin: 5px 0;
        }
        
        /* Mobile responsiveness */
        @media only screen and (max-width: 600px) {
            .email-container {
                width: 100% !important;
            }
            .content {
                padding: 15px !important;
            }
            .header h1 {
                font-size: 24px !important;
            }
            .price-highlight {
                font-size: 18px !important;
            }
        }
    </style>
</head>
<body>
    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
        <tr>
            <td align='center' style='padding: 20px 0;'>
                <table class='email-container' role='presentation' cellspacing='0' cellpadding='0' border='0'>
                    <!-- Header -->
                    <tr>
                        <td class='header'>
                            <img src='cid:logo' alt='TMS Logo' class='logo'>
                            <h1>הצעת מחיר</h1>
                            <p>מספר הצעה: #" + model.Offer.OfferId + @"</p>
                            <p>תאריך יצירה: " + model.Offer.CreatedAt.ToString("dd/MM/yyyy HH:mm") + @"</p>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td class='content'>");

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

            html.AppendLine("                            </div>");

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

            html.AppendLine("                            </div>");

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

            html.AppendLine("                            </div>");

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
                                    <h4>✓ כלול במחיר</h4>
                                    <ul>");

            if (!string.IsNullOrEmpty(model.Offer.PriceIncludes))
            {
                var includes = model.Offer.PriceIncludes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includes)
                {
                    html.AppendLine("                                        <li>" + HttpUtility.HtmlEncode(include.Trim()) + "</li>");
                }
            }
            else
            {
                html.AppendLine("                                        <li>לא צוינו פרטים</li>");
            }

            html.AppendLine(@"
                                    </ul>
                                </div>
                                
                                <div class='excludes'>
                                    <h4>✗ לא כלול במחיר</h4>
                                    <ul>");

            if (!string.IsNullOrEmpty(model.Offer.PriceExcludes))
            {
                var excludes = model.Offer.PriceExcludes.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var exclude in excludes)
                {
                    html.AppendLine("                                        <li>" + HttpUtility.HtmlEncode(exclude.Trim()) + "</li>");
                }
            }
            else
            {
                html.AppendLine("                                        <li>לא צוינו פרטים</li>");
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
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td class='footer'>
                            <p><strong>מערכת ניהול טיולים - TMS System</strong></p>
                            <p>הצעת מחיר זו תקפה למשך 30 יום מתאריך ההנפקה</p>
                            <p>לפרטים נוספים ניתן ליצור קשר בטלפון או באימייל</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>");

            return html.ToString();
        }

        public async Task SendOfferEmailAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default)
        {
            var offerId = model?.Offer?.OfferId ?? 0;
            _logger.LogInformation("הכנת מייל HTML להצעת מחיר #{OfferId} לשליחה אל {ToEmail}", offerId, toEmail);

            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("נדרשת כתובת אימייל", nameof(toEmail));

            // יצירת ה-HTML
            var htmlBody = await GenerateOfferEmailHtmlAsync(model);

            // יצירת המסר עם MimeKit
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpOptions.FromName ?? "TMS", _smtpOptions.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = $"הצעת מחיר #{offerId} - TMS";

            // יצירת ה-body עם תמונות מוטמעות
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlBody;
            builder.TextBody = $"הצעת מחיר #{offerId}\nלצפייה בהצעה, אנא פתח מייל זה בתוכנה התומכת ב-HTML.";

            // הוספת לוגו כתמונה מוטמעת
            var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
            if (File.Exists(logoPath))
            {
                var logo = builder.LinkedResources.Add(logoPath);
                logo.ContentId = "logo";
                _logger.LogInformation("לוגו נוסף כתמונה מוטמעת: {LogoPath}", logoPath);
            }
            else
            {
                _logger.LogWarning("לוגו לא נמצא בנתיב: {LogoPath}", logoPath);
            }

            message.Body = builder.ToMessageBody();

            // שליחת המייל
            using var client = new SmtpClient();
            try
            {
                _logger.LogInformation("מתחבר ל-SMTP {Host}:{Port}", _smtpOptions.Host, _smtpOptions.Port);

                await client.ConnectAsync(
                    _smtpOptions.Host, 
                    _smtpOptions.Port,
                    _smtpOptions.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, 
                    ct);

                await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password, ct);

                _logger.LogInformation("שולח מייל אל {To}", toEmail);
                await client.SendAsync(message, ct);
                
                _logger.LogInformation("הצעת מחיר #{OfferId} נשלחה בהצלחה אל {ToEmail}", offerId, toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה בשליחת מייל להצעת מחיר #{OfferId} אל {ToEmail}", offerId, toEmail);
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true, ct);
            }
        }
    }
}
