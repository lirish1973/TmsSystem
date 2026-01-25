using TmsSystem.Models;

namespace TmsSystem.Services
{
    public interface ITripOfferPdfService
    {
        Task<byte[]> GenerateTripOfferPdfAsync(TripOffer offer);
        Task<string> GenerateTripOfferHtmlAsync(TripOffer offer);
    }
}
