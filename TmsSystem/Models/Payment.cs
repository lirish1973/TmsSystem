namespace TmsSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string? Method { get; set; } // <-- שונה ל nullable
        public bool IsAvailable { get; set; } = true;
    }
}