using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TmsSystem.Models
{
    public class Tour
    {

        [Key]
        public int TourId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // קשרים לטבלאות המשנה
        public ICollection<ItineraryItem> Schedule { get; set; } = new List<ItineraryItem>();

        public ICollection<TourInclude> Includes { get; set; } = new List<TourInclude>();
        public ICollection<TourExclude> Excludes { get; set; } = new List<TourExclude>();
    }
}
