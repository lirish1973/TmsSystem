using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace TmsSystem.Services
{
    public class GmailSmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<GmailSmtpEmailService> _logger;

        public GmailSmtpEmailService(IOptions<SmtpOptions> options, ILogger<GmailSmtpEmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendHtmlAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Destination email address is required", nameof(toEmail));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName ?? "TMS", _options.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = plainTextBody ?? "צפייה בהצעה בפורמט HTML נתמכת במרבית תוכנות הדוא\"ל."
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                _logger.LogInformation("Connecting to SMTP {Host}:{Port}", _options.Host, _options.Port);

                await client.ConnectAsync(_options.Host, _options.Port,
                    _options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, ct);

                await client.AuthenticateAsync(_options.Username, _options.Password, ct);

                _logger.LogInformation("Sending email to {To}", toEmail);
                await client.SendAsync(message, ct);
            }
            finally
            {
                await client.DisconnectAsync(true, ct);
            }
        }
    }
}