using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TmsSystem.Models;

public class ItineraryItem
{
    [Key]
    [Column("ItemId")]
    public int ItemId { get; set; }


    public int ItineraryId { get; set; }   // FK ל-Itinerary
    public Itinerary Itinerary { get; set; }  // קשר ל-Itinerary


    public int TourId { get; set; }   // FK ישיר ל-Tour
    public Tour Tour { get; set; }    // קשר ישיר ל-Tour

    public TimeSpan StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
}
