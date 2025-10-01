using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "הסיסמאות אינן תואמות.")]
        public string ConfirmPassword { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? CompanyName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}
