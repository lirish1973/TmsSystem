using System.ComponentModel.DataAnnotations;

namespace TmsSystem.Models
{
    public class TripPriceItem
    {
        [Key]
        public int Id { get; set; }

        public int TripId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        // Navigation - nullable to avoid implicit [Required] from nullable reference types
        public Trip? Trip { get; set; }
    }
}
