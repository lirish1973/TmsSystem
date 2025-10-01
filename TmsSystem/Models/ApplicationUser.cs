namespace TmsSystem.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? CompanyName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
