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
        [Column("METHOD")]
        [StringLength(255)]
        public string METHOD { get; set; }
    }
}