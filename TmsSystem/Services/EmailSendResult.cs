namespace TmsSystem.Models
{
    public class EmailSendResult
    {
        public bool Success { get; set; }
        public string SentTo { get; set; }
        public string Subject { get; set; }
        public DateTime SentAt { get; set; }
        public string Provider { get; set; }
        public string ErrorMessage { get; set; }
    }
}