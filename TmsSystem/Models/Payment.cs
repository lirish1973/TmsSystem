using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmsSystem.Models
{
    public class Payment
    {
      //  public int PaymentId { get; set; }
      //  public string? Method { get; set; } // <-- שונה ל nullable
        public bool IsAvailable { get; set; } = true;


        [Key]
        [Column("PaymentId")]
        public int PaymentId { get; set; }

        [Required]
        [Column("OrderId")]
        public int OrderId { get; set; }

        [Required]
        [Column("Amount")]
        [Precision(10, 2)]
        public decimal Amount { get; set; }

        [Required]
        [Column("PaymentDate")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Column("Method")]
        [StringLength(100)]
        public string? Method { get; set; }

        // Navigation property
       // [ForeignKey("OrderId")]
     //   public virtual Order Order { get; set; }

    }
}