using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TmsSystem.Models;

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

        // ✅ מתודה מעודכנת עם תמיכה ב-inline images
        public async Task SendHtmlAsync(
            string toEmail,
            string subject,
            string htmlBody,
            string? plainTextBody = null,
            Dictionary<string, byte[]>? inlineImages = null,
            CancellationToken ct = default)
        {
            using var msg = new MailMessage();
            msg.From = new MailAddress(_options.FromEmail, _options.FromName);
            msg.To.Add(new MailAddress(toEmail));
            msg.Subject = subject;

            // אם יש inline images, צריך להשתמש ב-AlternateView
            // אם יש inline images, צריך להשתמש ב-AlternateView
            if (inlineImages != null && inlineImages.Any())
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

                // הוספת inline images
                foreach (var image in inlineImages)
                {
                    var contentId = image.Key;
                    var imageBytes = image.Value;

                    var ms = new MemoryStream(imageBytes);
                    var linkedResource = new LinkedResource(ms, MediaTypeNames.Image.Jpeg)
                    {
                        ContentId = contentId,
                        TransferEncoding = TransferEncoding.Base64
                    };

                    htmlView.LinkedResources.Add(linkedResource);
                }

                msg.AlternateViews.Add(htmlView);
                _logger.LogInformation("Added {Count} inline images to email", inlineImages.Count);
            }
            else
            {
                // אם אין inline images, פשוט שלח HTML רגיל
                msg.Body = htmlBody;
                msg.IsBodyHtml = true;
            }

            using var smtp = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            _logger.LogInformation("Sending email to {To} via System.Net.Mail", toEmail);
            await smtp.SendMailAsync(msg, ct);
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
            using var msg = new MailMessage();
            msg.From = new MailAddress(_options.FromEmail, _options.FromName);
            msg.To.Add(new MailAddress(toEmail));
            msg.Subject = subject;
            msg.Body = htmlBody;
            msg.IsBodyHtml = true;

            // הוספת קבצים מצורפים
            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    var bytes = System.Convert.FromBase64String(attachment.Base64Content);
                    using var ms = new MemoryStream(bytes);
                    var mailAttachment = new Attachment(ms, attachment.FileName, attachment.MimeType);
                    msg.Attachments.Add(mailAttachment);
                }

                _logger.LogInformation("Added {Count} attachments to email", attachments.Count);
            }

            using var smtp = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            _logger.LogInformation("Sending email with attachments to {To} via System.Net.Mail", toEmail);
            await smtp.SendMailAsync(msg, ct);
        }
    }
}