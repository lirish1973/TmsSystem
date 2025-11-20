using System.Net.Mail;
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

    public async Task SendHtmlAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null, CancellationToken ct = default)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(_config["SendGrid:FromEmail"], _config["SendGrid:FromName"]);
        var to = new EmailAddress(toEmail);

        // var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextBody, htmlBody);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextBody ?? "", htmlBody);

        var response = await client.SendEmailAsync(msg, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid error: {response.StatusCode} - {error}");
        }
    }
}