namespace TmsSystem.Models
{

    public class TourBooking
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int TourId { get; set; }
        public string GroupName { get; set; }
        public string BookerName { get; set; }
        public int ParticipantsCount { get; set; }
        public string PickupLocation { get; set; }
        public DateTime TourDate { get; set; }
        public string GuideName { get; set; }
        public string SpecialRequests { get; set; }
        public bool IncludesLunch { get; set; }

        public Customer Customer { get; set; }
        public Tour Tour { get; set; }
    }
}
