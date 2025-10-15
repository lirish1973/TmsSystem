using System.Threading.Tasks;
using TmsSystem.ViewModels;

namespace TmsSystem.Services
{
    // הרחבה למחלקה הקיימת
    public partial class PdfService : IPdfService
    {
     //   public Task<string> GenerateOfferHtmlAsync(ShowOfferViewModel model)
    //   {
            // GenerateHtmlContent היא private במחלקה, אך זמינה כאן כי זה אותו type (partial)
    //       return GenerateHtmlContent(model);
    //   }
    }
}