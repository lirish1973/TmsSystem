# תיקון טקסט עברי הפוך ב-PDF / Hebrew PDF Fix

## הבעיה / Problem
טקסט עברי הופיע הפוך (במראה ראי) בקבצי PDF שנוצרו על ידי המערכת.
Hebrew text was appearing reversed (mirrored) in generated PDF files.

## הפתרון / Solution  
**Updated 2026-01-26:** הוסרנו את החבילה `itext7.pdfcalligraph` (שדורשת רישיון מסחרי) והחלפנו אותה בשימוש בסמני Unicode BiDi.

We removed the `itext7.pdfcalligraph` package (which requires a commercial license) and replaced it with Unicode BiDi markers for proper RTL text rendering.

### שימוש בסמני BiDi / Using BiDi Markers
הפתרון משתמש בתווי בקרה של Unicode לסימון טקסט RTL:
The solution uses Unicode control characters to mark RTL text:
- **RLE (U+202B)** - RIGHT-TO-LEFT EMBEDDING - מסמן תחילת טקסט RTL
- **PDF (U+202C)** - POP DIRECTIONAL FORMATTING - מסמן סיום טקסט RTL

כל טקסט עברי נעטף אוטומטית בסמנים אלו באמצעות פונקציות עזר:
All Hebrew text is automatically wrapped with these markers using helper functions:
- `WrapRtlText()` - בודק אם יש תווים עבריים ועוטף עם RLE...PDF
- `EncodeAndWrapRtl()` - מקודד HTML ועוטף טקסט RTL

## שינויים שבוצעו / Changes Made

### 1. הסרת חבילה / Package Removed
```xml
<!-- REMOVED: requires commercial license -->
<!-- <PackageReference Include="itext7.pdfcalligraph" Version="5.0.5" /> -->
```

### 2. תווים חדשים / New Unicode Characters
קבועים חדשים ב-PdfService.cs ו-TripOfferPdfService.cs:
New constants in PdfService.cs and TripOfferPdfService.cs:
```csharp
private const char RLE = '\u202B'; // RIGHT-TO-LEFT EMBEDDING
private const char PDF = '\u202C'; // POP DIRECTIONAL FORMATTING
```

### 3. פונקציות עזר / Helper Functions
```csharp
private string WrapRtlText(string? text)
{
    if (string.IsNullOrEmpty(text)) return string.Empty;
    
    // Check if text contains Hebrew characters (U+0590 to U+05FF)
    bool hasHebrew = text.Any(c => c >= 0x0590 && c <= 0x05FF);
    
    if (hasHebrew)
    {
        // Wrap with RLE...PDF to force RTL rendering
        return $"{RLE}{text}{PDF}";
    }
    
    return text;
}

private string EncodeAndWrapRtl(string? text, string defaultValue = "לא צוין")
{
    var textToUse = text ?? defaultValue;
    var encoded = HttpUtility.HtmlEncode(textToUse);
    return WrapRtlText(encoded);
}
```

### 4. קבצים שעודכנו / Updated Files
- `TmsSystem/TmsSystem.csproj` - removed pdfCalligraph package
- `TmsSystem/Services/PdfService.cs` - added BiDi helper methods and error handling
- `TmsSystem/Services/TripOfferPdfService.cs` - added BiDi helper methods and error handling

## איך זה עובד / How It Works

### Automatic RTL Detection
המערכת בודקת אוטומטית טקסט עברי ועוטפת אותו:
The system automatically detects Hebrew text and wraps it:

1. ✅ זיהוי אוטומטי של תווים עבריים (U+0590-U+05FF) / Automatic Hebrew character detection
2. ✅ עטיפה עם RLE...PDF לכפיית כיוון RTL / Wrapping with RLE...PDF to force RTL
3. ✅ קידוד HTML בטוח / Safe HTML encoding  
4. ✅ עובד עם הגדרות CSS קיימות / Works with existing CSS settings

### Existing RTL CSS
התבניות כבר מכילות הגדרות RTL נכונות:
The templates already contain correct RTL settings:

```html
<html dir='rtl' lang='he'>
<body style='direction: rtl; text-align: right; unicode-bidi: embed;'>
```

### What Gets Wrapped
כל התוכן הבא נעטף אוטומטית בסמני BiDi:
All the following content is automatically wrapped with BiDi markers:
- שמות לקוחות / Customer names
- כתובות / Addresses
- כותרות טיולים / Tour titles
- תיאורים / Descriptions
- פרטי מדריכים / Guide details
- לוח זמנים / Schedule items
- רשימות כלול/לא כלול / Includes/Excludes lists
- בקשות מיוחדות / Special requests
- שמות שיטות תשלום / Payment method names

## יתרונות הפתרון / Solution Benefits

✅ **ללא רישיון נדרש** / No license required - פתרון חינמי לחלוטין / Completely free solution
✅ **עובד מיידית** / Works immediately - אין צורך בהגדרות נוספות / No additional configuration needed
✅ **ביצועים טובים** / Good performance - קל משקל יותר מ-pdfCalligraph / Lighter than pdfCalligraph
✅ **אמין** / Reliable - תואם לתקן Unicode BiDi / Unicode BiDi standard compliant

## טיפול בשגיאות / Error Handling

הוספנו טיפול משופר בשגיאות:
Added improved error handling:
```csharp
try
{
    HtmlConverter.ConvertToPdf(html, memoryStream, converterProperties);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to generate PDF...");
    throw new InvalidOperationException($"Failed to generate PDF: {ex.Message}", ex);
}
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
- ✅ ה-PDF נפתח ונשמר בהצלחה / PDF opens and saves successfully

## קישורים שימושיים / Useful Links
- [Unicode BiDi Algorithm](https://unicode.org/reports/tr9/)
- [iText7 Documentation](https://itextpdf.com/products/itext-7)
- [HTML dir attribute](https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/dir)

## תמיכה / Support
לשאלות או בעיות נוספות, פנה למפתח המערכת.
For questions or issues, contact the system developer.

---
תאריך עדכון / Update Date: 2026-01-26  
גרסה / Version: 2.0 - Unicode BiDi Solution
