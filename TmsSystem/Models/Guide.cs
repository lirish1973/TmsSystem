using System.ComponentModel.DataAnnotations;

namespace TmsSystem.Models
{
    public class Guide
    {
        public int GuideId { get; set; }

        [Required(ErrorMessage = "שם המדריך נדרש")]
        [StringLength(255)]
        public string GuideName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Phone(ErrorMessage = "מספר טלפון לא תקין")]
        [StringLength(50)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        [StringLength(255)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>(); // ✅ שינוי כאן
        public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>(); // ✅ שינוי כאן
    }
}