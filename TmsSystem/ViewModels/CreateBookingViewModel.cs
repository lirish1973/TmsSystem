using System;
using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class CreateBookingViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int TourId { get; set; }

        public string GroupName { get; set; }
        public string BookerName { get; set; }

        [Required]
        public int ParticipantsCount { get; set; }

        public string PickupLocation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TourDate { get; set; }

        public string GuideName { get; set; }
        public string SpecialRequests { get; set; }
        public bool IncludesLunch { get; set; }
    }
}
