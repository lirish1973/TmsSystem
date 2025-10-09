using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TmsSystem.ViewModels;

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