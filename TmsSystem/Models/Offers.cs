using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    public class Offer
    {
        [Key]
        public int OfferId { get; set; }

        // לקוח
        [Required]
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        // מדריך
        [Required]
        public int GuideId { get; set; }
        [ForeignKey("GuideId")]
        public Guide GuideName { get; set; }

        // סיור
        [Required]
        public int TourId { get; set; }
        [ForeignKey("TourId")]
        public Tour Tour { get; set; }

        // פרטי הצעה
        [Required]
        public int Participants { get; set; }

        [Required]
        public DateTime TripDate { get; set; }

        [StringLength(500)]
        public string PickupLocation { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(2000)]
        public string PriceIncludes { get; set; }

        [StringLength(2000)]
        public string PriceExcludes { get; set; }

        [StringLength(2000)]
        public string SpecialRequests { get; set; }

        public bool LunchIncluded { get; set; }

        public DateTime TourDate { get; set; }

        public decimal TotalPayment { get; set; }


        // אמצעי תשלום
        [Required]
        public int PaymentId { get; set; }
        [ForeignKey("PaymentId")]
       // public Payments Payments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
