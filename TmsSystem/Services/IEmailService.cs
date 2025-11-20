using System.Threading;
using System.Threading.Tasks;
using TmsSystem.Models;


namespace TmsSystem.Services
{
    public interface IEmailService
    {
        Task SendHtmlAsync(
            string toEmail,
            string subject,
            string htmlBody,
            string? plainTextBody = null,
            Dictionary<string, byte[]>? inlineImages = null,
            CancellationToken ct = default);
    }
}