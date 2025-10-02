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

        // שינוי הקישור - הסר את ForeignKey או שנה אותו
        // אפשרות 1: בלי navigation property
        // אפשרות 2: עם navigation property לטבלת payments
        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }

        // אם אתה צריך גישה ל-PaymentMethod, הוסף property נפרד:
        [NotMapped]
        public int PaymentMethodId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}