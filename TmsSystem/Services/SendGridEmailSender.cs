using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TmsSystem.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendHtmlAsync(email, subject, htmlMessage);
        }

        public async Task SendHtmlAsync(string email, string subject, string htmlMessage, string? textMessage = null, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var senderEmail = _configuration["SendGrid:SenderEmail"];
            var senderName = _configuration["SendGrid:SenderName"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("SendGrid API Key not configured. Logging email instead.");
                _logger.LogInformation($"EMAIL - To: {email}, Subject: {subject}");
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, textMessage ?? htmlMessage, htmlMessage);

            var response = await client.SendEmailAsync(msg, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"Email sent successfully to {email}");
            }
            else
            {
                _logger.LogError($"Failed to send email to {email}. Status: {response.StatusCode}");
            }
        }
    }
}