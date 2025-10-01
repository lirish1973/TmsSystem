using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class CreateOfferViewModel
    {
        [Required]
        public int CustomerId { get; set; } // לקוח שנבחר

        public List<CustomerSelectViewModel> Customers { get; set; } = new();

        [Required]
        public int Participants { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TourDate { get; set; }

        public string PickupLocation { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string PriceIncludes { get; set; }
        public string PriceExcludes { get; set; }

        [Required]
        public int GuideId { get; set; }
        public List<GuideSelectViewModel> Guides { get; set; } = new();

        public decimal TotalPayment { get; set; }

        public string SpecialRequests { get; set; }

        public bool LunchIncluded { get; set; }

        [Required]
        public int PaymentId { get; set; }
        public List<PaymentSelectViewModel> Payments { get; set; } = new();

        public int TourId { get; set; }  // חובה למלא את השדה


        public int PaymentMethodId { get; set; } // השדה שנבחר בפורם

        public List<PaymentMethodSelectViewModel> PaymentMethods { get; set; } = new(); // הרשימה של כל אמצעי התשלום

        public class PaymentMethodSelectViewModel
        {
            public int PaymentMethodId { get; set; }
            public string PaymentName { get; set; }
        }
        public class CustomerSelectViewModel
        {
            public int CustomerId { get; set; }
            public string DisplayName { get; set; } // לדוגמה: שם + טלפון
            public List<CustomerSelectViewModel> Customers { get; set; } = new();
        }

        public class GuideSelectViewModel
        {
            public int GuideId { get; set; }
            public string GuideName { get; set; }
        }

        public class PaymentSelectViewModel
        {
            public int PaymentId { get; set; }
            public string PaymentName { get; set; }
        }



    }
} 

