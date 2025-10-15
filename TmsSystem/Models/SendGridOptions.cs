public class SendGridOptions
{
    public const string SectionName = "SendGrid";

    public string ApiKey { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
}