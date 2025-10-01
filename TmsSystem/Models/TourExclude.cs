namespace TmsSystem.Models
{
    public class TourExclude
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string Description { get; set; }

        public string Text { get; set; } // Include / Exclude
        public Tour Tour { get; set; }
    }
}

