# מערכת שליחת מיילי HTML מתקדמת

## סקירה כללית
מערכת זו מספקת שליחת מיילי HTML מעוצבים ומקצועיים עם תמיכה בתמונות מוטמעות (embedded images) באמצעות MimeKit ו-MailKit.

## רכיבי המערכת

### 1. IHtmlEmailService
ממשק השירות לשליחת מיילי HTML:
```csharp
public interface IHtmlEmailService
{
    Task SendOfferEmailAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default);
    Task<string> GenerateOfferEmailHtmlAsync(ShowOfferViewModel model);
}
```

### 2. HtmlEmailService
המימוש של שירות שליחת HTML עם תמיכה בתמונות מוטמעות:

#### תכונות עיקריות:
- **תבנית HTML רספונסיבית** - מותאמת לתצוגה במכשירי מובייל
- **תמיכה בתמונות מוטמעות** - שימוש ב-CID (Content-ID) להטמעת לוגו ותמונות נוספות
- **עיצוב מקצועי** - גרדיאנטים, צבעים מותאמים, ועיצוב מודרני
- **תאימות למיילים** - שימוש בטבלאות HTML לתאימות מירבית עם לקוחות דוא"ל

#### מבנה ה-HTML:
```
┌─────────────────────────────┐
│  Header עם לוגו מוטמע       │
│  ├─ לוגו (CID embedded)    │
│  ├─ כותרת                   │
│  └─ מספר הצעה ותאריך        │
├─────────────────────────────┤
│  פרטי הלקוח                 │
│  ├─ שם מלא                  │
│  ├─ טלפון                   │
│  ├─ אימייל                  │
│  └─ כתובת                   │
├─────────────────────────────┤
│  פרטי הטיול                 │
│  ├─ שם הטיול                │
│  ├─ תאריך                   │
│  ├─ מספר משתתפים            │
│  └─ נקודת איסוף             │
├─────────────────────────────┤
│  פרטי המדריך                │
├─────────────────────────────┤
│  תיאור הטיול                │
├─────────────────────────────┤
│  פרטי מחיר (מודגש)          │
│  ├─ מחיר לאדם               │
│  ├─ סה"כ לתשלום             │
│  ├─ שיטת תשלום              │
│  └─ ארוחת צהריים            │
├─────────────────────────────┤
│  כלול במחיר / לא כלול       │
│  (בעיצוב צבעוני)            │
├─────────────────────────────┤
│  בקשות מיוחדות              │
├─────────────────────────────┤
│  Footer                     │
│  └─ פרטי יצירת קשר          │
└─────────────────────────────┘
```

### 3. OfferEmailSender
שירות המשמש כשכבת ביניים לשליחת הצעות מחיר:
```csharp
public class OfferEmailSender
{
    private readonly IHtmlEmailService _htmlEmailService;
    
    public async Task SendOfferEmailAsync(ShowOfferViewModel model, string toEmail, CancellationToken ct = default)
    {
        await _htmlEmailService.SendOfferEmailAsync(model, toEmail, ct);
    }
}
```

## הגדרות SMTP

### appsettings.json / appsettings.Development.json:
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UseStartTls": true,
    "Username": "your_account@gmail.com",
    "Password": "your_app_password_here",
    "FromEmail": "your_account@gmail.com",
    "FromName": "TMS"
  }
}
```

### הגדרות Gmail:
1. יצירת **App Password** (לא סיסמת החשבון הרגילה):
   - כניסה לחשבון Google
   - Security → 2-Step Verification
   - App passwords → יצירת סיסמה חדשה
2. שימוש ב-App Password בשדה `Password`

## רישום שירותים ב-Program.cs

```csharp
// הגדרות SMTP
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

// שירותי דוא"ל
builder.Services.AddSingleton<IEmailService, GmailSmtpEmailService>();
builder.Services.AddScoped<IHtmlEmailService, HtmlEmailService>();
builder.Services.AddScoped<OfferEmailSender>();
```

## שימוש במערכת

### בקונטרולר:
```csharp
[HttpPost]
public async Task<IActionResult> SendOfferEmail(int id, string email)
{
    var offer = await _context.Offers
        .Include(o => o.Customer)
        .Include(o => o.Guide)
        .Include(o => o.Tour)
        .Include(o => o.PaymentMethod)
        .FirstOrDefaultAsync(o => o.OfferId == id);

    var model = new ShowOfferViewModel
    {
        Offer = offer,
        PaymentMethod = offer.PaymentMethod
    };

    await _offerEmailSender.SendOfferEmailAsync(model, email);
    
    return Json(new { success = true, message = "הצעת המחיר נשלחה בהצלחה" });
}
```

## תמונות מוטמעות

### הוספת לוגו:
הלוגו נמצא ב: `wwwroot/images/logo.png`

השירות מטמיע אותו אוטומטית באמצעות:
```csharp
var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
if (File.Exists(logoPath))
{
    var logo = builder.LinkedResources.Add(logoPath);
    logo.ContentId = "logo";
}
```

ב-HTML:
```html
<img src='cid:logo' alt='TMS Logo' class='logo'>
```

### הוספת תמונות נוספות:
1. העלאת התמונה ל-`wwwroot/images/`
2. הוספת הקוד:
```csharp
var imagePath = Path.Combine(_environment.WebRootPath, "images", "image-name.png");
if (File.Exists(imagePath))
{
    var image = builder.LinkedResources.Add(imagePath);
    image.ContentId = "unique-image-id";
}
```
3. שימוש ב-HTML:
```html
<img src='cid:unique-image-id' alt='Description'>
```

## עיצוב ה-HTML

### צבעים עיקריים:
- **כחול ראשי**: `#2c5aa0` (כותרות, גבולות)
- **כחול כהה**: `#1a365d` (גרדיאנט)
- **ירוק**: `#28a745` (כלול במחיר)
- **אדום**: `#dc3545` (לא כלול במחיר)
- **תכלת**: `#17a2b8` (מחירים)

### רספונסיביות:
המערכת כוללת מדיה קוורי למובייל:
```css
@media only screen and (max-width: 600px) {
    .email-container { width: 100% !important; }
    .content { padding: 15px !important; }
    .header h1 { font-size: 24px !important; }
}
```

## תאימות

### תומך ב:
- ✅ Gmail
- ✅ Outlook
- ✅ Apple Mail
- ✅ Thunderbird
- ✅ מרבית לקוחות הדוא"ל המודרניים

### טיפים לתאימות:
1. שימוש בטבלאות HTML במקום DIV (עבור Outlook)
2. Inline CSS או `<style>` בתוך `<head>`
3. הימנעות מ-JavaScript
4. שימוש ב-web-safe fonts (Arial, Tahoma)
5. תמיכה ב-RTL עבור עברית

## לוגים ואבחון

השירות כולל לוגים מפורטים:
```
✅ הכנת מייל HTML להצעת מחיר #123 לשליחה אל customer@example.com
✅ לוגו נוסף כתמונה מוטמעת: /path/to/logo.png
✅ מתחבר ל-SMTP smtp.gmail.com:587
✅ שולח מייל אל customer@example.com
✅ הצעת מחיר #123 נשלחה בהצלחה אל customer@example.com
```

## טיפול בשגיאות

```csharp
try
{
    await _offerEmailSender.SendOfferEmailAsync(model, email);
}
catch (Exception ex)
{
    _logger.LogError(ex, "שגיאה בשליחת הצעת מחיר #{OfferId}", offerId);
    // טיפול בשגיאה...
}
```

## שיפורים עתידיים אפשריים

1. **תבניות נוספות** - תמיכה בסוגי מיילים שונים (אישורים, תזכורות)
2. **התאמה אישית** - אפשרות לבחור עיצוב או צבעים
3. **מצורפים** - הוספת PDF או קבצים נוספים
4. **מעקב** - tracking pixels למעקב אחר פתיחת מיילים
5. **רב-לשוניות** - תמיכה בשפות נוספות

## קבצים רלוונטיים

- `Services/IHtmlEmailService.cs` - ממשק השירות
- `Services/HtmlEmailService.cs` - מימוש השירות
- `Services/OfferEmailSender.cs` - שכבת ביניים לשליחת הצעות
- `Services/SmtpOptions.cs` - הגדרות SMTP
- `wwwroot/images/logo.png` - לוגו החברה
- `Program.cs` - רישום שירותים

## תמיכה ועזרה

לשאלות או בעיות:
1. בדיקת הלוגים במערכת
2. אימות הגדרות SMTP
3. בדיקת קיום הלוגו בנתיב הנכון
4. אימות הרשאות Gmail (App Password)
