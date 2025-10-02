using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    public class Offer
    {
        [Key]
        public int OfferId { get; set; }

        [Required(ErrorMessage = "יש לבחור לקוח")]
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required(ErrorMessage = "יש לבחור מדריך")]
        public int GuideId { get; set; }
        [ForeignKey("GuideId")]
        public Guide Guide { get; set; } // שינוי מ-GuideName

        [Required(ErrorMessage = "יש לבחור סיור")]
        public int TourId { get; set; }
        [ForeignKey("TourId")]
        public Tour Tour { get; set; }

        [Required(ErrorMessage = "יש להזין מספר משתתפים")]
        [Range(1, 100, ErrorMessage = "מספר המשתתפים חייב להיות בין 1 ל-100")]
        public int Participants { get; set; }

        // שני השדות כמו שמוגדרים בטבלה
        [Required(ErrorMessage = "יש לבחור תאריך טיול")]
        public DateTime TripDate { get; set; }

        [Required(ErrorMessage = "יש לבחור תאריך טיול")]
        public DateTime TourDate { get; set; }

        [StringLength(500)]
        public string PickupLocation { get; set; }

        [Required(ErrorMessage = "יש להזין מחיר")]
        [Range(0.01, double.MaxValue, ErrorMessage = "המחיר חייב להיות גדול מאפס")]
        public decimal Price { get; set; }

        public decimal TotalPayment { get; set; }

        [StringLength(2000)]
        public string PriceIncludes { get; set; }

        [StringLength(2000)]
        public string PriceExcludes { get; set; }

        [StringLength(2000)]
        public string SpecialRequests { get; set; }

        public bool LunchIncluded { get; set; }

        [Required(ErrorMessage = "יש לבחור אמצעי תשלום")]
        public int PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}