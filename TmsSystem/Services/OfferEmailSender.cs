using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TmsSystem.ViewModels;
using TmsSystem.Models;

namespace TmsSystem.Services
{
    public class OfferEmailSender
    {
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly ILogger<OfferEmailSender> _logger;

        public OfferEmailSender(IPdfService pdfService, IEmailService emailService, ILogger<OfferEmailSender> logger)
        {
            _pdfService = pdfService;
            _emailService = emailService;
            _logger = logger;
        }

        // הפונקציה החדשה שמחזירה EmailResponse
        public async Task<EmailResponse> SendOfferEmailWithResponseAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default)
        {
            var offerId = model?.Offer?.OfferId ?? 0;
            var subject = $"הצעת מחיר #{offerId} - TMS";
            var customerName = model?.Offer?.Customer?.FullName ?? model?.Offer?.Customer?.CustomerName ?? "";

            _logger.LogInformation("Preparing HTML for offer #{OfferId} to send to {ToEmail}", offerId, toEmail);

            try
            {
                var html = await _pdfService.GenerateOfferHtmlAsync(model);

                // שמירת זמן השליחה לפני השליחה
                var sentAt = DateTime.Now;

                // שליחת המייל עם הממשק הקיים (שמחזיר Task void)
                await _emailService.SendHtmlAsync(toEmail, subject, html, ct: ct);

                _logger.LogInformation("Offer #{OfferId} sent successfully to {ToEmail}", offerId, toEmail);

                return new EmailResponse
                {
                    Success = true,
                    SentTo = toEmail,
                    Subject = subject,
                    SentAt = sentAt,
                    Provider = "SendGrid",
                    CustomerName = customerName,
                    OfferId = offerId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending offer #{OfferId} to {ToEmail}", offerId, toEmail);
                return new EmailResponse
                {
                    Success = false,
                    SentTo = toEmail,
                    Subject = subject,
                    SentAt = DateTime.Now,
                    Provider = "SendGrid",
                    ErrorMessage = ex.Message,
                    CustomerName = customerName,
                    OfferId = offerId
                };
            }
        }

        // שמירה על הפונקציה הישנה לתאימות לאחור - בלי שינוי!
        public async Task SendOfferEmailAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default)
        {
            var offerId = model?.Offer?.OfferId ?? 0;
            _logger.LogInformation("Preparing HTML for offer #{OfferId} to send to {ToEmail}", offerId, toEmail);

            var html = await _pdfService.GenerateOfferHtmlAsync(model);
            await _emailService.SendHtmlAsync(toEmail, $"הצעת מחיר #{offerId} - TMS", html, ct: ct);

            _logger.LogInformation("Offer #{OfferId} sent to {ToEmail}", offerId, toEmail);
        }
    }
}