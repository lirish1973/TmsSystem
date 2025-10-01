namespace TmsSystem.Models
{
    public class TourInclude
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string Text { get; set; } // Include / Exclude
        public string Description { get; set; }

        public Tour Tour { get; set; }
    }

}
