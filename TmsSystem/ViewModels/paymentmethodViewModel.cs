using TmsSystem.Models;

namespace TmsSystem.ViewModels
{
    // public int PaymentMethodId { get; set; }
    // public List<PaymentMethodSelectViewModel> PaymentMethods { get; set; } = new();

    public class PaymentMethodViewModel
    {
   
     
        public int id { get; set; }

        public int PaymentMethodid { get; set; }


        public string PaymentMethodname { get; set; }

        public string method { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; } = new();
    
    }
}