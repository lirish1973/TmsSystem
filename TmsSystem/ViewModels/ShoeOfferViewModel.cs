using TmsSystem.Models;

namespace TmsSystem.ViewModels
{
    public class ShowOfferViewModel
    {
        public Offer Offer { get; set; }
      //  public PaymentMethod PaymentMethod { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public int? PaymentMethodId { get; set; } // הוסף ? להפוך ל-nullable
    }
}