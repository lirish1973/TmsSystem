using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    public class Trip
    {
        public int TripId { get; set; }

        [Required(ErrorMessage = "כותרת הטיול נדרשת")]
        [StringLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "מספר ימים נדרש")]
        [Range(1, 365, ErrorMessage = "מספר הימים חייב להיות בין 1 ל-365")]
        public int NumberOfDays { get; set; }

        // 🆕 שדה חדש - מדריך
        public int? GuideId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PricePerPerson { get; set; }

        [StringLength(500)]
        public string? PriceDescription { get; set; }

        public string? Includes { get; set; }
        public string? Excludes { get; set; }
        public string? FlightDetails { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<TripDay>? TripDays { get; set; }
       // public virtual ICollection<TripOffer>? TripOffers { get; set; }

        // 🆕 Navigation למדריך
        [ForeignKey("GuideId")]
        public virtual Guide? Guide { get; set; }
    }
}