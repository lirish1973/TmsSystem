namespace TmsSystem.Services
{
    public sealed class SmtpOptions
    {
        // ברירת מחדל לג׳ימייל
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool UseStartTls { get; set; } = true;

        // יש למלא בקובץ ההגדרות (appsettings/Secrets/Environment)
        public string Username { get; set; } = ""; // כתובת Gmail המלאה
        public string Password { get; set; } = ""; // App Password (לא סיסמת חשבון רגילה)
        public string FromEmail { get; set; } = ""; // בד"כ זהה ל-Username
        public string FromName { get; set; } = "TRYIT";
    }
}