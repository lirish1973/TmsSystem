using System.ComponentModel.DataAnnotations;

namespace TmsSystem.Models
{
    public class TripPriceItem
    {
        [Key]
        public int Id { get; set; }

        public int TripId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        // Navigation - nullable to avoid implicit [Required] from nullable reference types
        public Trip? Trip { get; set; }
    }
}
