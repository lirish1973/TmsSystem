using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TmsSystem.Services
{
    public class GmailNetSmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<GmailNetSmtpEmailService> _logger;

        public GmailNetSmtpEmailService(IOptions<SmtpOptions> options, ILogger<GmailNetSmtpEmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendHtmlAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null, Dictionary<string, byte[]>? inlineImages = null, CancellationToken ct = default)
        {
            using var msg = new MailMessage();
            msg.From = new MailAddress(_options.FromEmail, _options.FromName);
            msg.To.Add(new MailAddress(toEmail));
            msg.Subject = subject;
            msg.Body = htmlBody;
            msg.IsBodyHtml = true;

            using var smtp = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            _logger.LogInformation("Sending email to {To} via System.Net.Mail", toEmail);
            await smtp.SendMailAsync(msg);
        }
    }
}