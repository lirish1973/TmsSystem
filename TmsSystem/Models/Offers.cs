using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("offers")]
    public class Offer
    {
        [Key]
        public int OfferId { get; set; } 

        [Required]
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required]
        public int GuideId { get; set; }
        [ForeignKey("GuideId")]
        public Guide Guide { get; set; }

        [Required]
        public int TourId { get; set; }
        [ForeignKey("TourId")]
        public Tour Tour { get; set; }

        [Required]
        public int Participants { get; set; }

        [Required]
        public DateTime TripDate { get; set; }

        [Required]
        public DateTime TourDate { get; set; }

        [StringLength(500)]
        public string PickupLocation { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public decimal TotalPayment { get; set; }

        [StringLength(2000)]
        public string PriceIncludes { get; set; } = string.Empty;

        [StringLength(2000)]
        public string PriceExcludes { get; set; } = string.Empty;

        [StringLength(2000)]
        public string SpecialRequests { get; set; } = string.Empty;

        public bool LunchIncluded { get; set; } = false;

        // הסר את [Required] והפוך ל-nullable
        public int? PaymentId { get; set; }

        // הפוך ל-nullable
        [ForeignKey("PaymentId")]
        public Payment? Payment { get; set; }

        // PaymentMethodId כבר nullable - זה בסדר
        public int? PaymentMethodId { get; set; }


        [ForeignKey("PaymentMethodId")]
        public PaymentMethod? PaymentMethod { get; set; }

      

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}