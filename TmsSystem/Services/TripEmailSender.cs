using Microsoft.Extensions.Logging;
using TmsSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TmsSystem.Services
{
    public class TripEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<TripEmailSender> _logger;
        private readonly IConfiguration _configuration;

        private const string DEFAULT_EMAIL_PROVIDER = "SendGrid";
        private const string DEFAULT_CUSTOMER_NAME = "לקוח יקר";

        public TripEmailSender(
            IEmailService emailService,
            ILogger<TripEmailSender> logger,
            IConfiguration configuration)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<EmailResponse> SendTripProposalAsync(
            Trip trip,
            string toEmail,
            string customerName = null,
            CancellationToken ct = default)
        {
            if (trip == null)
            {
                _logger.LogError("Trip is null");
                throw new ArgumentNullException(nameof(trip));
            }

            var tripId = trip.TripId;
            var recipient = SanitizeCustomerName(customerName);

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogWarning("No email address provided for Trip #{TripId}", tripId);
                return CreateFailureResponse(toEmail, null, "כתובת אימייל לא נמצאה", recipient, tripId);
            }

            if (!IsValidEmail(toEmail))
            {
                _logger.LogWarning("Invalid email format: {Email} for Trip #{TripId}", toEmail, tripId);
                return CreateFailureResponse(toEmail, null, "כתובת אימייל לא תקינה", recipient, tripId);
            }

            var subject = $"הצעת טיול - {trip.Title} - TRYIT";

            try
            {
                _logger.LogInformation("Preparing trip proposal email for Trip #{TripId} to {ToEmail}", tripId, toEmail);

                var html = GenerateTripProposalHtml(trip, recipient);

                await _emailService.SendHtmlAsync(toEmail, subject, html, ct: ct);

                _logger.LogInformation("Trip proposal #{TripId} sent successfully to {ToEmail}", tripId, toEmail);

                return CreateSuccessResponse(toEmail, subject, recipient, tripId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Email sending was cancelled for Trip #{TripId}", tripId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending trip proposal #{TripId} to {ToEmail}", tripId, toEmail);
                return CreateFailureResponse(toEmail, subject, ex.Message, recipient, tripId);
            }
        }

        private string SanitizeCustomerName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DEFAULT_CUSTOMER_NAME;

            var sanitized = name.Trim();
            return sanitized.Length > 100 ? sanitized.Substring(0, 100) : sanitized;
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

        private EmailResponse CreateSuccessResponse(string email, string subject, string customerName, int tripId)
        {
            return new EmailResponse
            {
                Success = true,
                SentTo = email,
                Subject = subject,
                SentAt = DateTime.UtcNow,
                Provider = _configuration["EmailProvider"] ?? DEFAULT_EMAIL_PROVIDER,
                CustomerName = customerName,
                TripId = tripId
            };
        }

        private EmailResponse CreateFailureResponse(string email, string subject, string errorMessage, string customerName, int tripId)
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
                TripId = tripId
            };
        }

        private string HtmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return HttpUtility.HtmlEncode(text);
        }

        private string HtmlEncodeWithLineBreaks(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return HttpUtility.HtmlEncode(text).Replace("\n", "<br/>").Replace("\r", "");
        }

        private string GenerateTripProposalHtml(Trip trip, string customerName)
        {
            var sb = new StringBuilder(10240);

            var orderedDays = trip.TripDays?.OrderBy(d => d.DayNumber).ToList() ?? new List<TripDay>();
            var includesList = ParseMultilineText(trip.Includes);
            var excludesList = ParseMultilineText(trip.Excludes);
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://tryit.co.il";

            sb.Append($@"
<!DOCTYPE html>
<html dir='rtl' lang='he'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>הצעת טיול - {HtmlEncode(trip.Title)}</title>
    <style>{GetTripEmailCss()}</style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <div class='company-logo'>
                <h1>TRYIT</h1>
                <p class='tagline'>חוויות טיולים בלתי נשכחות</p>
            </div>
        </div>
        
        <div class='greeting'>
            <h2>שלום {HtmlEncode(customerName)},</h2>
            <p>שמחים להציג בפניך את הטיול המיוחד שלנו</p>
        </div>

        <div class='trip-header'>
            <h1 class='trip-title'>{HtmlEncode(trip.Title)}</h1>
            <div class='trip-info-grid'>
                <div class='info-item'>
                    <span class='icon'>📅</span>
                    <div>
                        <div class='label'>משך הטיול</div>
                        <div class='value'>{trip.NumberOfDays} ימים</div>
                    </div>
                </div>");

            if (trip.PricePerPerson > 0)
            {
                sb.Append($@"
                <div class='info-item'>
                    <span class='icon'>💰</span>
                    <div>
                        <div class='label'>מחיר לאדם</div>
                        <div class='value'>${trip.PricePerPerson:N2}</div>
                    </div>
                </div>");
            }

            sb.Append(@"
            </div>");

            if (!string.IsNullOrWhiteSpace(trip.Description))
            {
                sb.Append($@"
            <p class='trip-description'>{HtmlEncodeWithLineBreaks(trip.Description)}</p>");
            }

            sb.Append(@"</div>");

            // Daily Itinerary
            if (orderedDays.Any())
            {
                sb.Append(@"
        <div class='section-card'>
            <h2 class='section-title'>📋 תוכנית הטיול - יום אחר יום</h2>
        </div>
        <div class='days-container'>");

                foreach (var day in orderedDays)
                {
                    sb.Append($@"
            <div class='day-card'>
                <div class='day-header'>
                    <span class='day-number'>יום {day.DayNumber}</span>
                    <h3 class='day-title'>{HtmlEncode(day.Title)}</h3>
                </div>");

                    if (!string.IsNullOrWhiteSpace(day.Location))
                    {
                        sb.Append($@"
                <div class='day-location'>
                    <span class='icon'>📍</span>
                    <span>{HtmlEncode(day.Location)}</span>
                </div>");
                    }

                    sb.Append("<div class='day-content-grid'>");

                    if (!string.IsNullOrWhiteSpace(day.ImagePath))
                    {
                        sb.Append($@"
                    <div class='day-image-container'>
                        <img src='{baseUrl}{day.ImagePath}' alt='{HtmlEncode(day.Title)}' class='day-image' />
                    </div>");
                    }

                    if (!string.IsNullOrWhiteSpace(day.Description))
                    {
                        sb.Append($@"
                    <div class='day-description'>
                        {HtmlEncodeWithLineBreaks(day.Description)}
                    </div>");
                    }

                    sb.Append("</div></div>");
                }

                sb.Append("</div>");
            }

            // Includes
            if (includesList.Any())
            {
                sb.Append(@"
        <div class='section-card includes'>
            <h2 class='section-title'>✅ המחיר כולל</h2>
            <ul class='includes-list'>");
                foreach (var item in includesList)
                {
                    sb.Append($"<li>{HtmlEncode(item)}</li>");
                }
                sb.Append("</ul></div>");
            }

            // Excludes
            if (excludesList.Any())
            {
                sb.Append(@"
        <div class='section-card excludes'>
            <h2 class='section-title'>❌ המחיר אינו כולל</h2>
            <ul class='excludes-list'>");
                foreach (var item in excludesList)
                {
                    sb.Append($"<li>{HtmlEncode(item)}</li>");
                }
                sb.Append("</ul></div>");
            }

            // Flight Details
            if (!string.IsNullOrWhiteSpace(trip.FlightDetails))
            {
                sb.Append($@"
        <div class='section-card'>
            <h2 class='section-title'>✈️ פרטי טיסה</h2>
            <div class='content-box'>
                {HtmlEncodeWithLineBreaks(trip.FlightDetails)}
            </div>
        </div>");
            }

            sb.Append(@"
        <div class='footer'>
            <div class='contact-info'>
                <h3>צור קשר</h3>
                <p>📧 ofnoacomps@gmail.com</p>
                <p>📱 לפרטים נוספים ולהזמנת הטיול</p>
            </div>
            <div class='footer-note'>
                <p>מחכים לנסוע איתכם!</p>
                <p class='company-name'>TRYIT - חוויות טיולים בלתי נשכחות</p>
            </div>
        </div>
    </div>
</body>
</html>");

            return sb.ToString();
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

        private string GetTripEmailCss()
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
.trip-header { padding: 40px; background: linear-gradient(to bottom, #ffffff 0%, #f8f9fa 100%); }
.trip-title { font-size: 36px; color: #333; margin-bottom: 25px; font-weight: bold; text-align: center; }
.trip-info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin: 25px 0; }
.info-item { display: flex; align-items: center; gap: 12px; background: white; padding: 15px; border-radius: 10px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08); }
.info-item .icon { font-size: 32px; }
.info-item .label { font-size: 13px; color: #666; margin-bottom: 3px; }
.info-item .value { font-size: 18px; font-weight: 600; color: #333; }
.trip-description { font-size: 16px; color: #555; line-height: 1.8; margin-top: 20px; text-align: justify; }
.section-card { margin: 30px; }
.section-title { font-size: 24px; font-weight: bold; color: #667eea; margin-bottom: 20px; padding-bottom: 10px; border-bottom: 2px solid #667eea; }
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
@media (max-width: 768px) {
    .day-content-grid { grid-template-columns: 1fr; }
    .day-image-container { width: 100%; height: 250px; }
}";
        }
    }
}