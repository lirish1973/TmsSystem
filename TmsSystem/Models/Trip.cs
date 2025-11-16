using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("trips")]  // ← הוסף את זה
    public class Trip
    {
        [Key]
        [Column("TripId")]
        public int TripId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("Title")]
        public string Title { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Range(5, 12)]
        [Column("NumberOfDays")]
        public int NumberOfDays { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // Navigation Property
        [InverseProperty("Trip")]
        public virtual List<TripDay> TripDays { get; set; } = new List<TripDay>();
    }
}