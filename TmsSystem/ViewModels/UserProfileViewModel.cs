using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "שם משתמש הוא שדה חובה")]
        public string Username { get; set; }

        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט אימייל לא תקין")]
        public string Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? CompanyName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }

        // שדות לשינוי סיסמה
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [StringLength(100, ErrorMessage = "הסיסמה חייבת להיות לפחות {2} תווים", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "הסיסמאות אינן תואמות")]
        public string? ConfirmPassword { get; set; }
    }
}