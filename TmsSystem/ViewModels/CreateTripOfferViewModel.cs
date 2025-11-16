using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TmsSystem.Models;

namespace TmsSystem.ViewModels
{
    public class CreateTripOfferViewModel
    {
        [Required(ErrorMessage = "נא לבחור לקוח")]
        [Display(Name = "לקוח")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "נא לבחור טיול")]
        [Display(Name = "טיול")]
        public int TripId { get; set; }

        [Required(ErrorMessage = "נא להזין מספר משתתפים")]
        [Range(1, 100, ErrorMessage = "מספר המשתתפים חייב להיות בין 1 ל-100")]
        [Display(Name = "מספר משתתפים")]
        public int Participants { get; set; }

        [Required(ErrorMessage = "נא להזין תאריך יציאה")]
        [Display(Name = "תאריך יציאה")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "תאריך חזרה")]
        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "נא להזין מחיר לאדם")]
        [Range(0.01, 999999, ErrorMessage = "המחיר חייב להיות חיובי")]
        [Display(Name = "מחיר לאדם ($)")]
        public decimal PricePerPerson { get; set; }

        [Display(Name = "תוספת חדר יחיד")]
        [Range(0, 999999, ErrorMessage = "התוספת חייבת להיות חיובית")]
        public decimal? SingleRoomSupplement { get; set; }

        [Display(Name = "כמה חדרים יחידים")]
        [Range(0, 50, ErrorMessage = "מספר החדרים חייב להיות בין 0 ל-50")]
        public int SingleRooms { get; set; }

        [Required(ErrorMessage = "נא לבחור אמצעי תשלום")]
        [Display(Name = "אמצעי תשלום")]
        public int PaymentMethodId { get; set; }

        [Display(Name = "מספר תשלומים")]
        [Range(1, 36, ErrorMessage = "מספר התשלומים חייב להיות בין 1 ל-36")]
        public int? PaymentInstallments { get; set; }

        [Display(Name = "טיסה כלולה")]
        public bool FlightIncluded { get; set; }

        [Display(Name = "פרטי טיסה")]
        public string? FlightDetails { get; set; }

        [Display(Name = "ביטוח כלול")]
        public bool InsuranceIncluded { get; set; }

        [Display(Name = "מחיר ביטוח לאדם")]
        [Range(0, 999999, ErrorMessage = "מחיר הביטוח חייב להיות חיובי")]
        public decimal? InsurancePrice { get; set; }

        [Display(Name = "בקשות מיוחדות")]
        [StringLength(2000, ErrorMessage = "הבקשות המיוחדות לא יכולות לעלות על 2000 תווים")]
        public string? SpecialRequests { get; set; }

        [Display(Name = "הערות נוספות")]
        [StringLength(2000, ErrorMessage = "ההערות לא יכולות לעלות על 2000 תווים")]
        public string? AdditionalNotes { get; set; }

        // רשימות לבחירה
        public List<Customer>? Customers { get; set; }
        public List<Trip>? Trips { get; set; }
        public List<PaymentMethod>? PaymentMethods { get; set; }
    }
}