using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TmsSystem.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(ISendGridClient sendGridClient, IConfiguration configuration, ILogger<SendGridEmailService> logger)
        {
            _sendGridClient = sendGridClient;
            _configuration = configuration;
            _logger = logger;
        }

        // מממש את האינטרפייס הקיים
        public async Task SendHtmlAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null, CancellationToken ct = default)
        {
            try
            {
                var fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@example.com";
                var fromName = _configuration["SendGrid:FromName"] ?? "מערכת הגשת הצעות מחיר - TRYIT";

                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(toEmail);

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextBody, htmlBody);

                var response = await _sendGridClient.SendEmailAsync(msg, ct);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                }
                else
                {
                    _logger.LogWarning("Failed to send email to {ToEmail}. Status: {StatusCode}", toEmail, response.StatusCode);
                    throw new Exception($"Failed to send email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
                throw;
            }
        }
        // הוסף את המתודה החסרה
        public async Task<bool> SendQuoteEmailAsync(string toEmail, string customerName, string quoteContent)
        {
            try
            {
                var fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@example.com";
                var fromName = _configuration["SendGrid:FromName"] ?? "משרד הטיולים";

                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(toEmail, customerName);
                var subject = "הצעת מחיר - משרד הטיולים";

                var htmlContent = $@"
            <div dir='rtl' style='font-family: Arial, sans-serif;'>
                <h2>שלום {customerName},</h2>
                <p>מצורפת הצעת המחיר שביקשת:</p>
                <div style='border: 1px solid #ddd; padding: 20px; margin: 20px 0; background-color: #f9f9f9;'>
                    {quoteContent}
                </div>
                <p>תודה שבחרת בנו!</p>
                <p>בברכה,<br>צוות משרד הטיולים</p>
            </div>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation("Quote email sent successfully to {ToEmail}", toEmail);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to send quote email to {ToEmail}. Status: {StatusCode}", toEmail, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send quote email to {ToEmail}", toEmail);
                return false;
            }
        }


        public async Task SendHtmlAsync(string toEmail, string subject, string htmlContent, CancellationToken ct = default)
        {
            try
            {
                var fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@example.com";
                var fromName = _configuration["SendGrid:FromName"] ?? "מערכת הגשת הצעות מחיר - TRYIT";

                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(toEmail);

                var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

                var response = await _sendGridClient.SendEmailAsync(msg, ct);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                }
                else
                {
                    _logger.LogWarning("Failed to send email to {ToEmail}. Status: {StatusCode}", toEmail, response.StatusCode);
                    throw new Exception($"Failed to send email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
                throw;
            }
        }
    }
}