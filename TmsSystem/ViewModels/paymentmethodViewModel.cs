using TmsSystem.Models;

namespace TmsSystem.ViewModels
{
    // public int PaymentMethodId { get; set; }
    // public List<PaymentMethodSelectViewModel> PaymentMethods { get; set; } = new();

    public class PaymentMethodViewModel
    {
        [Key]
     
        public int id { get; set; }

        [Required]
        [MaxLength(255)]

        public string method { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; } = new();
    
    }
}