
using System.Collections.Generic;
using System.Linq;
using TmsSystem.Services;
using TmsSystem.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SendGridEmailService(IConfiguration config)
    {
        _config = config;
    }

    // ✅ מתודה מעודכנת עם תמיכה ב-inline images - אבל לא שוברת קוד קיים!
    public async Task SendHtmlAsync(
        string toEmail,
        string subject,
        string htmlBody,
        string? plainTextBody = null,
        Dictionary<string, byte[]>? inlineImages = null,
        CancellationToken ct = default)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(_config["SendGrid:FromEmail"], _config["SendGrid:FromName"]);
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextBody ?? "", htmlBody);

        // 🆕 הוספת inline images אם קיימות
        if (inlineImages != null && inlineImages.Any())
        {
            foreach (var image in inlineImages)
            {
                var contentId = image.Key;
                var imageBytes = image.Value;
                var base64Image = Convert.ToBase64String(imageBytes);

                // SendGrid מצפה לפורמט ספציפי של inline images
                msg.AddAttachment(new Attachment
                {
                    Content = base64Image,
                    ContentId = contentId,
                    Disposition = "inline",
                    Filename = $"{contentId}.jpg",
                    Type = "image/jpeg"
                });
            }
        }

        var response = await client.SendEmailAsync(msg, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid error: {response.StatusCode} - {error}");
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
        var apiKey = _config["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(_config["SendGrid:FromEmail"], _config["SendGrid:FromName"]);
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextBody ?? "", htmlBody);

        // הוספת קבצים מצורפים
        if (attachments != null && attachments.Any())
        {
            foreach (var attachment in attachments)
            {
                msg.AddAttachment(new Attachment
                {
                    Content = attachment.Base64Content,
                    Filename = attachment.FileName,
                    Type = attachment.MimeType,
                    Disposition = "attachment"
                });
            }
        }

        var response = await client.SendEmailAsync(msg, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid error: {response.StatusCode} - {error}");
        }
    }
}