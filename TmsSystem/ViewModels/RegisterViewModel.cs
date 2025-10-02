using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "שם משתמש נדרש")]
        [Display(Name = "שם משתמש")]
        public string Username { get; set; }

        [Required(ErrorMessage = "אימייל נדרש")]
        [EmailAddress(ErrorMessage = "אימייל לא תקין")]
        [Display(Name = "אימייל")]
        public string Email { get; set; }

        [Required(ErrorMessage = "סיסמה נדרשת")]
        [StringLength(100, ErrorMessage = "הסיסמה חייבת להכיל לפחות {2} תווים", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמה")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "אישור סיסמה")]
        [Compare("Password", ErrorMessage = "הסיסמאות לא זהות")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "שם פרטי")]
        public string? FirstName { get; set; }

        [Display(Name = "שם משפחה")]
        public string? LastName { get; set; }

        [Display(Name = "טלפון")]
        public string? Phone { get; set; }

        [Display(Name = "כתובת")]
        public string? Address { get; set; }

        [Display(Name = "שם חברה")]
        public string? CompanyName { get; set; }

        [Display(Name = "תאריך לידה")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "תפקיד")]
        public string? Role { get; set; } = "User"; // ברירת מחדל
    }
}