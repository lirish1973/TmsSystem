using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("tripoffers")]
    public class TripOffer
    {
        [Key]
        [Column("TripOfferId")]
        public int TripOfferId { get; set; }

        [Required]
        [Column("CustomerId")]
        public int CustomerId { get; set; }

        [Required]
        [Column("TripId")]
        public int TripId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("OfferNumber")]
        public string OfferNumber { get; set; } = string.Empty;

        [Column("OfferDate")]
        public DateTime OfferDate { get; set; } = DateTime.Now;

        [Required]
        [Column("Participants")]
        public int Participants { get; set; }

        [Required]
        [Column("DepartureDate")]
        public DateTime DepartureDate { get; set; }

        [Column("ReturnDate")]
        public DateTime? ReturnDate { get; set; }

        [Required]
        [Column("PricePerPerson", TypeName = "decimal(10,2)")]
        public decimal PricePerPerson { get; set; }

        [Column("SingleRoomSupplement", TypeName = "decimal(10,2)")]
        public decimal? SingleRoomSupplement { get; set; }

        [Column("SingleRooms")]
        public int SingleRooms { get; set; }

        [Required]
        [Column("TotalPrice", TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column("PaymentMethodId")]
        [Display(Name = "אמצעי תשלום")]
        public int PaymentMethodId { get; set; }

        [Column("PaymentInstallments")]
        public int? PaymentInstallments { get; set; }

        [Column("FlightIncluded")]
        public bool FlightIncluded { get; set; }

        [Column("FlightDetails")]
        public string? FlightDetails { get; set; }

        [Column("InsuranceIncluded")]
        public bool InsuranceIncluded { get; set; }

        [Column("InsurancePrice", TypeName = "decimal(10,2)")]
        public decimal? InsurancePrice { get; set; }

        [Column("SpecialRequests")]
        public string? SpecialRequests { get; set; }

        [Column("AdditionalNotes")]
        public string? AdditionalNotes { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("TripId")]
        public virtual Trip? Trip { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod? PaymentMethod { get; set; }
    }
}