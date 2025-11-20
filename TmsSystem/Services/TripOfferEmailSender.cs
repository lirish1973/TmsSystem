using Microsoft.Extensions.Logging;
using TmsSystem.Models;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace TmsSystem.Services
{
    public class TripOfferEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<TripOfferEmailSender> _logger;
        private readonly IConfiguration _configuration;

        private const string DEFAULT_EMAIL_PROVIDER = "SendGrid";
        private const int OFFER_VALIDITY_DAYS = 30;
        private const string DEFAULT_CUSTOMER_NAME = "לקוח יקר";

        public TripOfferEmailSender(
            IEmailService emailService,
            ILogger<TripOfferEmailSender> logger,
            IConfiguration configuration)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<EmailResponse> SendTripOfferEmailAsync(
            TripOffer offer,
            string toEmail = null,
            CancellationToken ct = default)
        {
            if (offer == null)
            {
                _logger.LogError("TripOffer is null");
                throw new ArgumentNullException(nameof(offer));
            }

            var offerId = offer.TripOfferId;
            var customerName = SanitizeCustomerName(offer.Customer?.DisplayName);
            var recipientEmail = toEmail ?? offer.Customer?.Email;

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogWarning("No email address found for Offer #{OfferId}", offerId);
                return CreateFailureResponse(recipientEmail, null, "כתובת אימייל לא נמצאה", customerName, offerId);
            }

            if (!IsValidEmail(recipientEmail))
            {
                _logger.LogWarning("Invalid email format: {Email} for Offer #{OfferId}", recipientEmail, offerId);
                return CreateFailureResponse(recipientEmail, null, "כתובת אימייל לא תקינה", customerName, offerId);
            }

            var subject = BuildEmailSubject(offer);

            try
            {
                _logger.LogInformation("Preparing trip offer email for Offer #{OfferId} to {ToEmail}", offerId, recipientEmail);

                var html = GenerateTripOfferHtml(offer, customerName);

                //await _emailService.SendHtmlAsync(recipientEmail, subject, html, ct: ct);
                await _emailService.SendHtmlAsync(recipientEmail, subject, html, plainTextBody: "", ct: ct);

                _logger.LogInformation("Trip offer #{OfferId} sent successfully to {ToEmail}", offerId, recipientEmail);

                return CreateSuccessResponse(recipientEmail, subject, customerName, offerId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Email sending was cancelled for Offer #{OfferId}", offerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending trip offer #{OfferId} to {ToEmail}", offerId, recipientEmail);
                return CreateFailureResponse(recipientEmail, subject, ex.Message, customerName, offerId);
            }
        }

        private string SanitizeCustomerName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DEFAULT_CUSTOMER_NAME;

            var sanitized = name.Trim();
            return sanitized.Length > 100 ? sanitized.Substring(0, 100) : sanitized;
        }

        private string BuildEmailSubject(TripOffer offer)
        {
            var tripTitle = offer.Trip?.Title ?? "טיול";
            return $"הצעת מחיר מספר {offer.OfferNumber} - {tripTitle} - TRYIT";
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                if (!emailRegex.IsMatch(email))
                    return false;

                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private EmailResponse CreateSuccessResponse(string email, string subject, string customerName, int offerId)
        {
            return new EmailResponse
            {
                Success = true,
                SentTo = email,
                Subject = subject,
                SentAt = DateTime.UtcNow,
                Provider = _configuration["EmailProvider"] ?? DEFAULT_EMAIL_PROVIDER,
                CustomerName = customerName,
                TripOfferId = offerId
            };
        }

        private EmailResponse CreateFailureResponse(string email, string subject, string errorMessage, string customerName, int offerId)
        {
            return new EmailResponse
            {
                Success = false,
                SentTo = email,
                Subject = subject,
                SentAt = DateTime.UtcNow,
                Provider = _configuration["EmailProvider"] ?? DEFAULT_EMAIL_PROVIDER,
                ErrorMessage = errorMessage,
                CustomerName = customerName,
                TripOfferId = offerId
            };
        }

        private List<string> ParseMultilineText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            return text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(line => line.Trim())
                      .Where(line => !string.IsNullOrWhiteSpace(line))
                      .ToList();
        }

        private string HtmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return HttpUtility.HtmlEncode(text);
        }

        private string ConvertImageToBase64(string imagePath)
        {
            try
            {
                // אם זה URL מלא, נחזיר אותו כמו שהוא
                if (imagePath.StartsWith("http://") || imagePath.StartsWith("https://"))
                {
                    return imagePath;
                }

                // בונים את הנתיב המלא לקובץ
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var fullPath = Path.Combine(webRootPath, imagePath.TrimStart('/'));

                // בדיקה שהקובץ קיים
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("Image file not found: {Path}", fullPath);
                    return string.Empty;
                }

                // קריאת הקובץ והמרה ל-Base64
                byte[] imageBytes = File.ReadAllBytes(fullPath);
                string base64String = Convert.ToBase64String(imageBytes);

                // זיהוי סוג הקובץ
                string extension = Path.GetExtension(fullPath).ToLower();
                string mimeType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "image/jpeg"
                };

                return $"data:{mimeType};base64,{base64String}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting image to base64: {Path}", imagePath);
                return string.Empty;
            }
        }


        private string HtmlEncodeWithLineBreaks(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return HttpUtility.HtmlEncode(text).Replace("\n", "<br/>").Replace("\r", "");
        }

        private string GenerateTripOfferHtml(TripOffer model, string customerName)
        {
            var sb = new StringBuilder(10240);

            var orderedDays = model.Trip?.TripDays?.OrderBy(d => d.DayNumber).ToList() ?? new List<TripDay>();
            var includesList = ParseMultilineText(model.Trip?.Includes);
            var excludesList = ParseMultilineText(model.Trip?.Excludes);
            var validityDate = DateTime.UtcNow.AddDays(OFFER_VALIDITY_DAYS);
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7033";

            sb.Append($@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>הצעת מחיר - {HtmlEncode(model.Trip?.Title)}</title>
    <style>{GetTripOfferEmailCss()}</style>
</head>
<body dir='rtl'>
    <div class='email-container'>
        <div class='header'>
            <div class='company-logo'>
                <h1>TRYIT</h1>
                <p class='tagline'>חוויות טיולים בלתי נשכחות</p>
            </div>
        </div>
        <div class='greeting'>
            <h2>שלום {HtmlEncode(customerName)},</h2>
            <p>שמחים להציג בפניך את הצעת המחיר המפורטת לטיול המבוקש</p>
            <div class='offer-number'>הצעת מחיר מספר: <strong>{HtmlEncode(model.OfferNumber)}</strong></div>
        </div>
        <div class='trip-header'>
            <h1 class='trip-title'>{HtmlEncode(model.Trip?.Title)}</h1>
            <div class='trip-info-grid'>
                <div class='info-item'>
                    <span class='icon'>📅</span>
                    <div>
                        <div class='label'>משך הטיול</div>
                        <div class='value'>{model.Trip?.NumberOfDays ?? 0} ימים</div>
                    </div>
                </div>
                <div class='info-item'>
                    <span class='icon'>✈️</span>
                    <div>
                        <div class='label'>תאריך יציאה</div>
                        <div class='value'>{model.DepartureDate:dd/MM/yyyy}</div>
                    </div>
                </div>");

            if (model.ReturnDate.HasValue)
            {
                sb.Append($@"
                <div class='info-item'>
                    <span class='icon'>🏠</span>
                    <div>
                        <div class='label'>תאריך חזרה</div>
                        <div class='value'>{model.ReturnDate.Value:dd/MM/yyyy}</div>
                    </div>
                </div>");
            }

            sb.Append($@"
                <div class='info-item'>
                    <span class='icon'>👥</span>
                    <div>
                        <div class='label'>מספר משתתפים</div>
                        <div class='value'>{model.Participants} איש</div>
                    </div>
                </div>
            </div>");

            if (!string.IsNullOrWhiteSpace(model.Trip?.Description))
            {
                sb.Append($@"<p class='trip-description'>{HtmlEncodeWithLineBreaks(model.Trip.Description)}</p>");
            }

            sb.Append($@"</div>
        <div class='price-section'>
            <h2 class='section-title'>💰 פירוט מחירים</h2>
            <div class='price-breakdown'>
                <div class='price-row'><span>מחיר לאדם</span><span>${model.PricePerPerson:N2}</span></div>
                <div class='price-row'><span>מספר משתתפים</span><span>× {model.Participants}</span></div>
                <div class='price-row subtotal'><span>סכום ביניים</span><span>${(model.PricePerPerson * model.Participants):N2}</span></div>");

            if (model.SingleRooms > 0 && model.SingleRoomSupplement.HasValue)
            {
                sb.Append($@"<div class='price-row'><span>תוספת {model.SingleRooms} חדרים יחידים (${model.SingleRoomSupplement.Value:N2} לחדר)</span><span>${(model.SingleRoomSupplement.Value * model.SingleRooms):N2}</span></div>");
            }

            if (model.InsuranceIncluded && model.InsurancePrice.HasValue)
            {
                sb.Append($@"<div class='price-row'><span>ביטוח נסיעות ({model.Participants} משתתפים × ${model.InsurancePrice.Value:N2})</span><span>${(model.InsurancePrice.Value * model.Participants):N2}</span></div>");
            }

            sb.Append($@"<div class='price-row total'><span>סה""כ לתשלום</span><span>${model.TotalPrice:N2}</span></div>
            </div>
        </div>
        <div class='payment-section'>
            <h2 class='section-title'>💳 פרטי תשלום</h2>
            <div class='payment-grid'>
                <div class='payment-item'><strong>אמצעי תשלום:</strong> {HtmlEncode(model.PaymentMethod?.PaymentName ?? "לא צוין")}</div>");

            if (model.PaymentInstallments.HasValue)
            {
                sb.Append($@"<div class='payment-item'><strong>מספר תשלומים:</strong> {model.PaymentInstallments.Value} תשלומים</div>");
            }

            sb.Append($@"
                <div class='payment-item'><strong>טיסה:</strong> {(model.FlightIncluded ? "✅ כלולה במחיר" : "❌ לא כלולה")}</div>
                <div class='payment-item'><strong>ביטוח:</strong> {(model.InsuranceIncluded ? "✅ כלול במחיר" : "❌ לא כלול")}</div>
            </div>
        </div>");

            if (model.FlightIncluded && !string.IsNullOrWhiteSpace(model.FlightDetails))
            {
                sb.Append($@"<div class='section-card'><h2 class='section-title'>✈️ פרטי טיסה</h2><div class='content-box'>{HtmlEncodeWithLineBreaks(model.FlightDetails)}</div></div>");
            }

            if (orderedDays.Any())
            {
                sb.Append(@"<div class='section-card'><h2 class='section-title'>📋 תוכנית הטיול המלאה - יום אחר יום</h2></div><div class='days-container'>");

                foreach (var day in orderedDays)
                {
                    sb.Append($@"<div class='day-card'><div class='day-header'><span class='day-number'>יום {day.DayNumber}</span><h3 class='day-title'>{HtmlEncode(day.Title)}</h3></div>");

                    if (!string.IsNullOrWhiteSpace(day.Location))
                    {
                        sb.Append($@"<div class='day-location'><span class='icon'>📍</span><span>{HtmlEncode(day.Location)}</span></div>");
                    }

                    sb.Append("<div class='day-content-grid'>");

                    if (!string.IsNullOrWhiteSpace(day.ImagePath))
                    {
                        var base64Image = ConvertImageToBase64(day.ImagePath);
                        if (!string.IsNullOrEmpty(base64Image))
                        {
                            sb.Append($@"<div class='day-image-container'><img src='{base64Image}' alt='{HtmlEncode(day.Title)}' class='day-image' /></div>");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(day.Description))
                    {
                        sb.Append($@"<div class='day-description'>{HtmlEncodeWithLineBreaks(day.Description)}</div>");
                    }

                    sb.Append("</div></div>");
                }

                sb.Append("</div>");
            }

            if (includesList.Any())
            {
                sb.Append(@"<div class='section-card includes'><h2 class='section-title'>✅ המחיר כולל</h2><ul class='includes-list'>");
                foreach (var item in includesList) sb.Append($"<li>{HtmlEncode(item)}</li>");
                sb.Append("</ul></div>");
            }

            if (excludesList.Any())
            {
                sb.Append(@"<div class='section-card excludes'><h2 class='section-title'>❌ המחיר אינו כולל</h2><ul class='excludes-list'>");
                foreach (var item in excludesList) sb.Append($"<li>{HtmlEncode(item)}</li>");
                sb.Append("</ul></div>");
            }

            if (!string.IsNullOrWhiteSpace(model.SpecialRequests))
            {
                sb.Append($@"<div class='section-card'><h2 class='section-title'>⭐ בקשות מיוחדות</h2><div class='content-box'>{HtmlEncodeWithLineBreaks(model.SpecialRequests)}</div></div>");
            }

            if (!string.IsNullOrWhiteSpace(model.AdditionalNotes))
            {
                sb.Append($@"<div class='section-card'><h2 class='section-title'>📝 הערות נוספות</h2><div class='content-box'>{HtmlEncodeWithLineBreaks(model.AdditionalNotes)}</div></div>");
            }

            sb.Append($@"
        <div class='footer'>
            <div class='contact-info'>
                <h3>צור קשר</h3>
                <p>mailto:info@tryit.co.il</p>
                <p> לפרטים נוספים ולאישור הזמנה</p>
            </div>
            <div class='footer-note'>
                <p>מחכים לנסוע איתכם!</p>
                <p class='company-name'>TRYIT - חוויות טיולים בלתי נשכחות</p>
                <p class='validity'>הצעה זו בתוקף עד {validityDate:dd/MM/yyyy}</p>
            </div>
        </div>
    </div>
</body>
</html>");

            return sb.ToString();
        }

        private string GetTripOfferEmailCss()
        {
            return @"
* { margin: 0; padding: 0; box-sizing: border-box; }
body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f5f5; padding: 20px; direction: rtl; text-align: right; line-height: 1.6; }
.email-container { max-width: 800px; margin: 0 auto; background: white; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); }
.header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center; color: white; }
.company-logo h1 { font-size: 48px; font-weight: bold; margin-bottom: 10px; text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3); }
.tagline { font-size: 18px; opacity: 0.95; }
.greeting { padding: 30px; background: linear-gradient(to bottom, #f8f9fa 0%, #ffffff 100%); border-bottom: 3px solid #667eea; }
.greeting h2 { color: #333; margin-bottom: 10px; font-size: 26px; }
.greeting p { color: #666; font-size: 16px; margin-bottom: 15px; }
.offer-number { background: #667eea; color: white; padding: 12px 20px; border-radius: 25px; display: inline-block; font-size: 16px; margin-top: 10px; }
.trip-header { padding: 40px; background: linear-gradient(to bottom, #ffffff 0%, #f8f9fa 100%); }
.trip-title { font-size: 36px; color: #333; margin-bottom: 25px; font-weight: bold; text-align: center; }
.trip-info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin: 25px 0; }
.info-item { display: flex; align-items: center; gap: 12px; background: white; padding: 15px; border-radius: 10px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08); }
.info-item .icon { font-size: 32px; }
.info-item .label { font-size: 13px; color: #666; margin-bottom: 3px; }
.info-item .value { font-size: 18px; font-weight: 600; color: #333; }
.trip-description { font-size: 16px; color: #555; line-height: 1.8; margin-top: 20px; text-align: justify; }
.price-section, .payment-section { margin: 30px; }
.section-title { font-size: 24px; font-weight: bold; color: #667eea; margin-bottom: 20px; padding-bottom: 10px; border-bottom: 2px solid #667eea; }
.price-breakdown { background: #f8f9fa; border-radius: 12px; padding: 20px; box-shadow: 0 3px 10px rgba(0, 0, 0, 0.1); }
.price-row { display: flex; justify-content: space-between; padding: 12px 0; border-bottom: 1px solid #dee2e6; font-size: 16px; }
.price-row:last-child { border-bottom: none; }
.price-row.subtotal { font-weight: 600; color: #495057; margin-top: 8px; }
.price-row.total { background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 18px 20px; margin: 15px -20px -20px -20px; border-radius: 0 0 12px 12px; font-size: 22px; font-weight: bold; }
.payment-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 15px; background: #f8f9fa; padding: 20px; border-radius: 12px; }
.payment-item { padding: 12px; background: white; border-radius: 8px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.05); }
.section-card { margin: 30px; }
.content-box { background: #f8f9fa; padding: 20px; border-radius: 10px; line-height: 1.8; }
.days-container { padding: 20px; }
.day-card { background: white; border: 2px solid #e0e0e0; border-radius: 12px; margin-bottom: 25px; overflow: hidden; box-shadow: 0 3px 15px rgba(0, 0, 0, 0.1); }
.day-header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; display: flex; align-items: center; gap: 15px; }
.day-number { background: white; color: #667eea; padding: 8px 16px; border-radius: 20px; font-weight: bold; font-size: 16px; }
.day-title { font-size: 22px; font-weight: bold; margin: 0; flex: 1; }
.day-location { padding: 12px 20px; color: #666; font-size: 15px; background: #f8f9fa; }
.day-content-grid { display: grid; grid-template-columns: 350px 1fr; gap: 0; }
.day-image-container { width: 350px; height: 300px; overflow: hidden; }
.day-image { width: 100%; height: 100%; object-fit: cover; }
.day-description { padding: 20px; color: #555; line-height: 1.8; font-size: 15px; text-align: justify; }
.includes-list, .excludes-list { list-style: none; padding: 20px; background: #f8f9fa; border-radius: 10px; }
.includes-list li, .excludes-list li { padding: 12px 0 12px 30px; border-bottom: 1px solid #dee2e6; font-size: 16px; color: #333; position: relative; }
.includes-list li:before { content: '✓'; position: absolute; right: 0; color: #28a745; font-weight: bold; font-size: 18px; }
.excludes-list li:before { content: '✗'; position: absolute; right: 0; color: #dc3545; font-weight: bold; font-size: 18px; }
.includes-list li:last-child, .excludes-list li:last-child { border-bottom: none; }
.footer { background: #2c3e50; color: white; padding: 40px; text-align: center; }
.contact-info { margin-bottom: 30px; }
.contact-info h3 { font-size: 24px; margin-bottom: 15px; }
.contact-info p { font-size: 16px; margin: 8px 0; opacity: 0.9; }
.footer-note { border-top: 1px solid rgba(255, 255, 255, 0.2); padding-top: 30px; }
.footer-note p { margin: 10px 0; font-size: 16px; }
.company-name { font-weight: bold; font-size: 18px; opacity: 0.95; }
.validity { font-size: 14px; opacity: 0.8; margin-top: 15px; font-style: italic; }
@media (max-width: 768px) {
    .day-content-grid { grid-template-columns: 1fr; }
    .day-image-container { width: 100%; height: 250px; }
    .payment-grid { grid-template-columns: 1fr; }
}";
        }
    }
}