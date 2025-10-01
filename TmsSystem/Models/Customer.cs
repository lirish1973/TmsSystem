﻿
using System.ComponentModel.DataAnnotations;

namespace TmsSystem.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "תעודת זהות היא שדה חובה")]
        [StringLength(20)]
        public string IDNumber { get; set; }  // מספר ייחודי

        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "טלפון הוא שדה חובה")]
        [Phone]
        public string Phone { get; set; }

        public string? FullName { get; set; }       // <-- שונה ל nullable
        public string? CustomerName { get; set; }   // <-- שונה ל nullable
        public string? CompanyName { get; set; }    // <-- שונה ל nullable
        public string? Address { get; set; }        // <-- שונה ל nullable

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}