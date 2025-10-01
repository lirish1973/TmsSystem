namespace TmsSystem.Models
{
    public class Guide
    {
        public int GuideId { get; set; }
        public string? GuideName { get; set; }    // <-- שונה ל nullable
        public string? Description { get; set; }  // <-- שונה ל nullable
    }
}