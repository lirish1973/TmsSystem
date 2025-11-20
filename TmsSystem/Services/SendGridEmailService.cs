using TmsSystem.Services;
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

    public async Task SendHtmlAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null, Dictionary<string, (byte[] data, string filename)>? inlineImages = null, CancellationToken ct = default)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(_config["SendGrid:FromEmail"], _config["SendGrid:FromName"]);
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextBody ?? "", htmlBody);

        // הוספת תמונות inline
        if (inlineImages != null && inlineImages.Any())
        {
            foreach (var image in inlineImages)
            {
                var base64Image = Convert.ToBase64String(image.Value.data);
                
                // קביעת סוג ה-MIME על פי סיומת הקובץ
                var extension = Path.GetExtension(image.Value.filename).ToLower();
                var mimeType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    ".bmp" => "image/bmp",
                    _ => "image/jpeg"
                };
                
                var attachment = new Attachment
                {
                    Content = base64Image,
                    Filename = image.Value.filename,
                    Type = mimeType,
                    Disposition = "inline",
                    ContentId = image.Key
                };
                msg.AddAttachment(attachment);
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