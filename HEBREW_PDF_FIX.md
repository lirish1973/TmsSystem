# תיקון טקסט עברי הפוך ב-PDF / Hebrew PDF Fix

## הבעיה / Problem
טקסט עברי הופיע הפוך (במראה ראי) בקבצי PDF שנוצרו על ידי המערכת.
Hebrew text was appearing reversed (mirrored) in generated PDF files.

## הפתרון / Solution
הוספנו את החבילה `itext7.pdfcalligraph` - תוסף מתקדם של iText7 שמטפל באופן אוטומטי בטקסט RTL (ימין לשמאל) כמו עברית וערבית.

We added the `itext7.pdfcalligraph` package - an advanced iText7 add-on that automatically handles RTL (right-to-left) text like Hebrew and Arabic.

## שינויים שבוצעו / Changes Made

### 1. הוספת חבילה / Package Added
```xml
<PackageReference Include="itext7.pdfcalligraph" Version="5.0.5" />
```

### 2. ניקוי קוד / Code Cleanup
- הוסרו פונקציות עזר שלא היו בשימוש לסיבוב טקסט ידני
- Removed unused manual text reversal helper functions
- הפונקציות `ReverseHebrewSafely` ו-`ReverseHebrewCharsOnly` כבר לא נחוצות
- The `ReverseHebrewSafely` and `ReverseHebrewCharsOnly` functions are no longer needed

### 3. קבצים שעודכנו / Updated Files
- `TmsSystem/TmsSystem.csproj` - added pdfCalligraph package
- `TmsSystem/Services/PdfService.cs` - removed manual reversal code
- `TmsSystem/Services/TripOfferPdfService.cs` - added documentation

## איך זה עובד / How It Works

### Automatic RTL Detection
pdfCalligraph מזהה אוטומטית תווים עבריים ומעבד אותם נכון:
pdfCalligraph automatically detects Hebrew characters and processes them correctly:

1. ✅ זיהוי אוטומטי של טקסט RTL / Automatic RTL text detection
2. ✅ סדר לוגי נכון של תווים / Correct logical character order  
3. ✅ תמיכה בכיוון ימין לשמאל / Right-to-left direction support
4. ✅ עובד עם הגדרות CSS קיימות / Works with existing CSS settings

### Existing RTL CSS
התבניות כבר מכילות הגדרות RTL נכונות:
The templates already contain correct RTL settings:

```html
<html dir='rtl' lang='he'>
<body style='direction: rtl; text-align: right; unicode-bidi: embed;'>
```

## רישיון / Licensing

### מצב ניסיון / Trial Mode
- החבילה עובדת במצב ניסיון ללא רישיון
- The package works in trial mode without a license
- יכול להופיע סימן מים "Trial Version"
- May show "Trial Version" watermark
- ✅ פונקציונליות RTL מלאה
- ✅ Full RTL functionality

### רישיון מסחרי / Commercial License
לשימוש בפרודקשן בלי סימן מים:
For production use without watermark:

1. רכשו רישיון מ-iText: https://itextpdf.com/products/itext-7/calligraph
   Purchase license from iText: https://itextpdf.com/products/itext-7/calligraph

2. הגדירו את הרישיון כמשתנה סביבה:
   Set the license as environment variable:
   ```bash
   export ITEXT_LICENSE_FILE=/path/to/itextkey.json
   ```

## בדיקה / Testing

### 1. בנייה / Build
```bash
cd TmsSystem
dotnet build
```
✅ הפרויקט בנוי בהצלחה / Project builds successfully

### 2. יצירת PDF / Generate PDF
1. הפעילו את האפליקציה / Run the application
2. צרו הצעת מחיר עם טקסט עברי / Create an offer with Hebrew text
3. הורידו את ה-PDF / Download the PDF
4. בדקו שהטקסט העברי מופיע נכון (לא הפוך)
   Verify Hebrew text appears correctly (not reversed)

### 3. מה לבדוק / What to Check
- ✅ שמות לקוחות בעברית / Hebrew customer names
- ✅ תיאורי טיולים / Tour descriptions
- ✅ פרטים נוספים / Additional details
- ✅ כל טקסט עברי אחר / Any other Hebrew text

## טיפ לפתרון בעיות / Troubleshooting

### אם הטקסט עדיין הפוך / If text is still reversed:
1. ✅ ודאו שהחבילה הותקנה: `dotnet list package | grep pdfcalligraph`
   Verify package is installed: `dotnet list package | grep pdfcalligraph`

2. ✅ בדקו שהפרויקט נבנה בהצלחה
   Check project builds successfully

3. ✅ נקו ובנו מחדש: `dotnet clean && dotnet build`
   Clean and rebuild: `dotnet clean && dotnet build`

### אם יש סימן מים / If there's a watermark:
זה תקין - משמעות הדבר ש-pdfCalligraph עובד במצב ניסיון.
This is normal - it means pdfCalligraph is working in trial mode.

לשימוש בפרודקשן: רכשו רישיון מסחרי מ-iText.
For production use: purchase a commercial license from iText.

## קישורים שימושיים / Useful Links
- [iText pdfCalligraph Documentation](https://itextpdf.com/products/itext-7/calligraph)
- [NuGet Package](https://www.nuget.org/packages/itext7.pdfcalligraph/)
- [iText Licensing](https://itextpdf.com/how-buy)

## תמיכה / Support
לשאלות או בעיות נוספות, פנה למפתח המערכת.
For questions or issues, contact the system developer.

---
תאריך עדכון / Update Date: 2026-01-25
גרסה / Version: 1.0
