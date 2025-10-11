using System;
using System.Threading;
using System.Threading.Tasks;
using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
    public class OfferEmailSender
    {
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public OfferEmailSender(IPdfService pdfService, IEmailService emailService)
        {
            _pdfService = pdfService;
            _emailService = emailService;
        }

        public async Task SendOfferEmailAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default)
        {
            var offerId = model?.Offer?.OfferId ?? 0;
            Console.WriteLine($"Preparing HTML for offer #{offerId} to send to {toEmail}");

            var html = await _pdfService.GenerateOfferHtmlAsync(model);
            await _emailService.SendHtmlAsync(toEmail, $"הצעת מחיר #{offerId} - TMS", html, ct: ct);

            Console.WriteLine($"Offer #{offerId} sent to {toEmail}");
        }
    }
}