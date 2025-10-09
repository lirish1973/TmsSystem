using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateOfferPdfAsync(ShowOfferViewModel model);

        Task<string> GenerateOfferHtmlAsync(ShowOfferViewModel model);
    }
}