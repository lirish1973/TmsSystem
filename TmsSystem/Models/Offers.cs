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
        public Guide Guide { get; set; } // שינוי מ-GuideName

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
        public string PickupLocation { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal TotalPayment { get; set; }

        [StringLength(2000)]
        public string PriceIncludes { get; set; }

        [StringLength(2000)]
        public string PriceExcludes { get; set; }

        [StringLength(2000)]
        public string SpecialRequests { get; set; }

        public bool LunchIncluded { get; set; } = false;

        [Required]
        public int PaymentId { get; set; }
        [ForeignKey("PaymentMethodId")]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}