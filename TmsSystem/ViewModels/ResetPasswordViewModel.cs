using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "נא להזין כתובת אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public string Email { get; set; }

        [Required(ErrorMessage = "נא להזין סיסמה חדשה")]
        [StringLength(100, ErrorMessage = "הסיסמה חייבת להכיל לפחות {2} תווים", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמה חדשה")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "אימות סיסמה")]
        [Compare("Password", ErrorMessage = "הסיסמאות אינן תואמות")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}