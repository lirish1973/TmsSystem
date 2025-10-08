using System;
using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class Users
    {
        public string Id { get; set; }

        [Display(Name = "שם משתמש")]
        public string Username { get; set; }

        [Required(ErrorMessage = "יש להזין אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        [Display(Name = "אימייל")]
        public string Email { get; set; }

        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }

        [Display(Name = "שם משפחה")]
        public string LastName { get; set; }

        [Display(Name = "כתובת")]
        public string address { get; set; }

        [Phone(ErrorMessage = "מספר טלפון לא תקין")]
        [Display(Name = "טלפון")]
        public string Phone { get; set; }

        [Display(Name = "שם החברה")]
        public string CompanyName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "תאריך לידה")]
        public DateTime? BirthDate { get; set; }
    }
}