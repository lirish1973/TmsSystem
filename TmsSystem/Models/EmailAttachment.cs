namespace TmsSystem.Models
{
    public class EmailAttachment
    {
        public string FileName { get; set; }
        public string Base64Content { get; set; }
        public string MimeType { get; set; }
    }
}