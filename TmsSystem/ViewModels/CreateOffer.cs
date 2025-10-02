using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class CreateOfferViewModel
    {
        [Required(ErrorMessage = "יש לבחור לקוח")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "יש לבחור מדריך")]
        public int GuideId { get; set; }

        public int TourId { get; set; } = 1; // ברירת מחדל

        [Required(ErrorMessage = "יש להזין מספר משתתפים")]
        [Range(1, 100, ErrorMessage = "מספר המשתתפים חייב להיות בין 1 ל-100")]
        public int Participants { get; set; }

        [Required(ErrorMessage = "יש לבחור תאריך טיול")]
        [DataType(DataType.Date)]
        public DateTime TourDate { get; set; }

        public string PickupLocation { get; set; }

        [Required(ErrorMessage = "יש להזין מחיר")]
        [Range(0.01, double.MaxValue, ErrorMessage = "המחיר חייב להיות גדול מאפס")]
        public decimal Price { get; set; }

        public string PriceIncludes { get; set; }
        public string PriceExcludes { get; set; }
        public string SpecialRequests { get; set; }
        public bool LunchIncluded { get; set; }

        [Required(ErrorMessage = "יש לבחור אמצעי תשלום")]
        public int PaymentMethodId { get; set; }

        public List<TourSelectViewModel> Tours { get; set; } = new List<TourSelectViewModel>();

        // רשימות לבחירה
        public List<CustomerSelectViewModel> Customers { get; set; } = new();
        public List<GuideSelectViewModel> Guides { get; set; } = new();
        public List<PaymentMethodSelectViewModel> PaymentMethods { get; set; } = new();
    }



    public class CustomerSelectViewModel
    {
        public int CustomerId { get; set; }
        public string DisplayName { get; set; }
    }

    public class GuideSelectViewModel
    {
        public int GuideId { get; set; }
        public string GuideName { get; set; }
    }

    public class TourSelectViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
    }

    public class PaymentMethodSelectViewModel
    {
        public int PaymentMethodId { get; set; }
        public string PaymentName { get; set; }
    }
} 

