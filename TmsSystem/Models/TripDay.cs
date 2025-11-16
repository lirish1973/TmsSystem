using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("tripdays")]  // ← הוסף את זה
    public class TripDay
    {
        [Key]
        [Column("TripDayId")]
        public int TripDayId { get; set; }

        [Required]
        [Column("TripId")]
        public int TripId { get; set; }

        [Required]
        [Range(1, 12)]
        [Column("DayNumber")]
        public int DayNumber { get; set; }

        [Required]
        [StringLength(255)]
        [Column("Title")]
        public string Title { get; set; }

        [StringLength(500)]
        [Column("Location")]
        public string? Location { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [StringLength(500)]
        [Column("ImagePath")]
        public string? ImagePath { get; set; }

        [Column("DisplayOrder")]
        public int DisplayOrder { get; set; }

        // Navigation Property
        [ForeignKey("TripId")]
        public Trip? Trip { get; set; }
    }
}