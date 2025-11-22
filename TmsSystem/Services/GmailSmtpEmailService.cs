using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TmsSystem.Models;

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

        // ✅ מתודה מעודכנת עם תמיכה ב-inline images
        public async Task SendHtmlAsync(
            string toEmail,
            string subject,
            string htmlBody,
            string? plainTextBody = null,
            Dictionary<string, byte[]>? inlineImages = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Destination email address is required", nameof(toEmail));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName ?? "TRYIT", _options.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = plainTextBody ?? "צפייה בהצעה בפורמט HTML נתמכת במרבית תוכנות הדוא\"ל."
            };

            // 🆕 הוספת inline images
            if (inlineImages != null && inlineImages.Any())
            {
                foreach (var image in inlineImages)
                {
                    var contentId = image.Key;
                    var imageBytes = image.Value;

                    // יצירת MimePart עבור התמונה
                    var imagePart = builder.LinkedResources.Add(contentId, imageBytes);
                    imagePart.ContentId = contentId;

                    _logger.LogInformation("Added inline image: {ContentId}", contentId);
                }

                _logger.LogInformation("Added {Count} inline images to email", inlineImages.Count);
            }

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

        // 🆕 מתודה חדשה - שליחת מייל עם קבצים מצורפים
        public async Task SendHtmlWithAttachmentsAsync(
            string toEmail,
            string subject,
            string htmlBody,
            List<EmailAttachment> attachments,
            string? plainTextBody = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Destination email address is required", nameof(toEmail));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName ?? "TRYIT", _options.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = plainTextBody ?? "צפייה בהצעה בפורמט HTML נתמכת במרבית תוכנות הדוא\"ל."
            };

            // הוספת קבצים מצורפים
            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    var bytes = Convert.FromBase64String(attachment.Base64Content);
                    builder.Attachments.Add(attachment.FileName, bytes, ContentType.Parse(attachment.MimeType));

                    _logger.LogInformation("Added attachment: {FileName} ({MimeType})", attachment.FileName, attachment.MimeType);
                }

                _logger.LogInformation("Added {Count} attachments to email", attachments.Count);
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                _logger.LogInformation("Connecting to SMTP {Host}:{Port}", _options.Host, _options.Port);

                await client.ConnectAsync(_options.Host, _options.Port,
                    _options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, ct);

                await client.AuthenticateAsync(_options.Username, _options.Password, ct);

                _logger.LogInformation("Sending email with attachments to {To}", toEmail);
                await client.SendAsync(message, ct);
            }
            finally
            {
                await client.DisconnectAsync(true, ct);
            }
        }
    }
}