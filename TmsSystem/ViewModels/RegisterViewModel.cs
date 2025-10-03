using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "שם משתמש הוא שדה חובה")]
        public string Username { get; set; }

        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט אימייל לא תקין")]
        public string Email { get; set; }

        [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        [StringLength(100, ErrorMessage = "הסיסמה חייבת להיות לפחות {2} תווים", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "אישור סיסמה הוא שדה חובה")]
        [Compare("Password", ErrorMessage = "הסיסמאות אינן תואמות")]
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