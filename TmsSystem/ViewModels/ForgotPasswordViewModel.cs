using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "נא להזין כתובת אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        [Display(Name = "אימייל")]
        public string Email { get; set; }
    }
}