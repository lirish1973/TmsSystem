using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("hotels")]
    public class Hotel
    {
        [Key]
        [Column("HotelId")]
        public int HotelId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("HotelName")]
        [Display(Name = "שם בית המלון")]
        public string HotelName { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("Location")]
        [Display(Name = "מיקום")]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
