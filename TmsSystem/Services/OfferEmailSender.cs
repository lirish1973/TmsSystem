using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
    /// <summary>
    /// שירות לשליחת הצעות מחיר במייל
    /// משתמש בשירות HTML Email עם תמונות מוטמעות
    /// </summary>
    public class OfferEmailSender
    {
        private readonly IHtmlEmailService _htmlEmailService;
        private readonly ILogger<OfferEmailSender> _logger;

        public OfferEmailSender(IHtmlEmailService htmlEmailService, ILogger<OfferEmailSender> logger)
        {
            _htmlEmailService = htmlEmailService;
            _logger = logger;
        }

        public async Task SendOfferEmailAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default)
        {
            var offerId = model?.Offer?.OfferId ?? 0;
            _logger.LogInformation("שליחת הצעת מחיר #{OfferId} למייל {ToEmail}", offerId, toEmail);

            // שימוש בשירות HTML Email המשודרג עם לוגו מוטמע
            await _htmlEmailService.SendOfferEmailAsync(model, toEmail, ct);

            _logger.LogInformation("הצעת מחיר #{OfferId} נשלחה למייל {ToEmail}", offerId, toEmail);
        }
    }
}