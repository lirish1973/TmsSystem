namespace TmsSystem.Models
{
    public class Itinerary
    {
        public int ItineraryId { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; }

        public Tour Tour { get; set; }
        public ICollection<ItineraryItem> Items { get; set; }
    }
}
