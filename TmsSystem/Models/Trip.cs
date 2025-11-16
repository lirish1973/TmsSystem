using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("trips")]
    public class Trip
    {
        [Key]
        [Column("TripId")]
        public int TripId { get; set; }

        [Required(ErrorMessage = "שם הטיול הוא שדה חובה")]
        [StringLength(255)]
        [Column("Title")]
        [Display(Name = "שם הטיול")]
        public string Title { get; set; }

        [Column("Description")]
        [Display(Name = "תיאור כללי")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "מספר ימים הוא שדה חובה")]
        [Range(5, 12, ErrorMessage = "מספר הימים חייב להיות בין 5 ל-12")]
        [Column("NumberOfDays")]
        [Display(Name = "מספר ימים")]
        public int NumberOfDays { get; set; }

        [Column("CreatedAt")]
        [Display(Name = "תאריך יצירה")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("IsActive")]
        [Display(Name = "פעיל")]
        public bool IsActive { get; set; } = true;

        // שדות חדשים
        [Column("PricePerPerson")]
        [Display(Name = "מחיר לאדם")]
        [Range(0, 999999, ErrorMessage = "המחיר חייב להיות חיובי")]
        public decimal? PricePerPerson { get; set; }

        [Column("PriceDescription")]
        [StringLength(500)]
        [Display(Name = "תיאור המחיר")]
        public string? PriceDescription { get; set; }

        [Column("Includes")]
        [Display(Name = "מה כלול במחיר")]
        public string? Includes { get; set; }

        [Column("Excludes")]
        [Display(Name = "מה לא כלול במחיר")]
        public string? Excludes { get; set; }

        [Column("FlightDetails")]
        [Display(Name = "פרטי טיסות")]
        public string? FlightDetails { get; set; }

        // Navigation Property
        [InverseProperty("Trip")]
        public virtual List<TripDay> TripDays { get; set; } = new List<TripDay>();
    }
}