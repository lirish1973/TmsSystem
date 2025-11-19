using System;

namespace TmsSystem.Models
{
    public class EmailResponse
    {
        public bool Success { get; set; }
        public string SentTo { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int OfferId { get; set; }
        public int? TripId { get; set; }
        public int? TripOfferId { get; set; }
    }
}