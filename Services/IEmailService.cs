using System.Threading.Tasks;

namespace Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
    }
}