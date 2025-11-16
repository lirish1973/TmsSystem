using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    [Table("paymentmethod")]
    public class PaymentMethod
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        [Column("METHOD")]
        public string METHOD { get; set; } = string.Empty;

        [Column("PaymentMethodId")]
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(140)]
        [Column("PaymentName")]
        public string PaymentName { get; set; } = string.Empty;

        // Property נוח יותר לשימוש
        [NotMapped]
        public string DisplayName => PaymentName;
    }
}