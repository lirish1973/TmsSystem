using System.Threading;
using System.Threading.Tasks;
using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
    /// <summary>
    /// שירות ליצירת ושליחת מיילי HTML עם תמונות מוטמעות
    /// </summary>
    public interface IHtmlEmailService
    {
        /// <summary>
        /// שליחת הצעת מחיר במייל HTML מעוצב עם לוגו מוטמע
        /// </summary>
        Task SendOfferEmailAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default);
        
        /// <summary>
        /// יצירת HTML להצעת מחיר מיועד למיילים (עם תמיכה בתמונות מוטמעות)
        /// </summary>
        Task<string> GenerateOfferEmailHtmlAsync(ShowOfferViewModel model);
    }
}
