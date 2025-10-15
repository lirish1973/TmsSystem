namespace TmsSystem.ViewModels
{
    public class EmailSuccessViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int OfferId { get; set; }
        public DateTime SentAt { get; set; }
    }
}