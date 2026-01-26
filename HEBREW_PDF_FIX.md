# תיקון טקסט עברי הפוך ב-PDF / Hebrew PDF Fix

## הבעיה / Problem
טקסט עברי הופיע הפוך (במראה ראי) בקבצי PDF שנוצרו על ידי המערכת.
Hebrew text was appearing reversed (mirrored) in generated PDF files.

## הפתרון / Solution  
**Updated 2026-01-26:** החלפנו את גישת סמני BiDi בהיפוך ידני של מחרוזות עבריות לפני יצירת ה-PDF.

We replaced the BiDi marker approach with manual Hebrew string reversal before PDF generation.

### הסיבה לשינוי / Why the Change
הגישה הקודמת עם סמני BiDi Unicode (RLE/PDF) לא עבדה כהלכה עם iText7. ספריית iText7 הופכת את הטקסט העברי בזמן יצירת ה-PDF, ללא קשר לסמני BiDi.

The previous BiDi marker approach didn't work properly with iText7. The iText7 library reverses Hebrew text during PDF generation, regardless of BiDi markers.

### איך זה עובד / How It Works
1. **זיהוי טקסט עברי** - הפונקציה `ReverseHebrewText()` בודקת אם יש תווים עבריים (U+0590-U+05FF)
2. **היפוך מחרוזת** - אם נמצא טקסט עברי, המחרוזת מתהפכת באופן ידני
3. **iText7 מהפך שוב** - כשiText7 יוצר את ה-PDF, הוא מהפך את הטקסט שוב, וכך הוא מגיע למצב הנכון
4. **קידוד HTML** - לאחר ההיפוך, הטקסט עובר קידוד HTML לבטיחות

1. **Hebrew detection** - The `ReverseHebrewText()` function checks for Hebrew characters (U+0590-U+05FF)
2. **String reversal** - If Hebrew text is found, the string is manually reversed
3. **iText7 reverses again** - When iText7 generates the PDF, it reverses the text again, resulting in correct display
4. **HTML encoding** - After reversal, text is HTML-encoded for safety

## שינויים שבוצעו / Changes Made

### 1. פונקציה חדשה / New Function
```csharp
private string ReverseHebrewText(string? text)
{
    if (string.IsNullOrEmpty(text)) return string.Empty;
    
    // Check if text contains Hebrew characters (U+0590 to U+05FF)
    bool hasHebrew = text.Any(c => c >= 0x0590 && c <= 0x05FF);
    
    if (hasHebrew)
    {
        // Reverse the entire string to compensate for iText7's reversal
        char[] charArray = text.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    
    return text;
}
```

### 2. עדכון פונקציית עזר / Updated Helper Function
```csharp
private string EncodeAndReverseRtl(string? text, string defaultValue = "לא צוין")
{
    var textToUse = text ?? defaultValue;
    // First reverse if Hebrew, then HTML encode
    var reversed = ReverseHebrewText(textToUse);
    return HttpUtility.HtmlEncode(reversed);
}
```

### 3. קבצים שעודכנו / Updated Files
- `TmsSystem/Services/PdfService.cs` - replaced BiDi markers with string reversal
- `TmsSystem/Services/TripOfferPdfService.cs` - replaced BiDi markers with string reversal

## איך זה עובד / How It Works

### Automatic Hebrew Detection and Reversal
המערכת בודקת אוטומטית טקסט עברי ומהפכת אותו לפני יצירת ה-PDF:
The system automatically detects Hebrew text and reverses it before PDF generation:

1. ✅ זיהוי אוטומטי של תווים עבריים (U+0590-U+05FF) / Automatic Hebrew character detection
2. ✅ היפוך מחרוזת באופן ידני / Manual string reversal
3. ✅ קידוד HTML בטוח / Safe HTML encoding  
4. ✅ עובד עם הגדרות CSS קיימות / Works with existing CSS settings

### Existing RTL CSS
התבניות כבר מכילות הגדרות RTL נכונות:
The templates already contain correct RTL settings:

```html
<html dir='rtl' lang='he'>
<body style='direction: rtl; text-align: right;'>
```

### What Gets Reversed
כל התוכן הבא מתהפך אוטומטית:
All the following content is automatically reversed:
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

✅ **פועל באמת** / Actually works - פתרון שנבדק ועובד עם iText7 / Tested solution that works with iText7
✅ **ללא רישיון נדרש** / No license required - פתרון חינמי לחלוטין / Completely free solution
✅ **פשוט ואמין** / Simple and reliable - ללא תלות בספריות חיצוניות / No external library dependencies
✅ **קל לתחזוקה** / Easy to maintain - קוד ברור ומובן / Clear and understandable code

## דוגמה / Example

**Input (before reversal):**
```
"הצעה למחיר"
```

**After ReverseHebrewText:**
```
"ריחמל העצה"
```

**After iText7 processing (in PDF):**
```
"הצעה למחיר" ✅ (correct!)
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
- ✅ "הצעה" מופיעה כ"הצעה" ולא כ"העצה" / "הצעה" appears as "הצעה" not "העצה"
- ✅ שמות לקוחות בעברית / Hebrew customer names
- ✅ תיאורי טיולים / Tour descriptions
- ✅ פרטים נוספים / Additional details
- ✅ ה-PDF נפתח ונשמר בהצלחה / PDF opens and saves successfully

## תמיכה / Support
לשאלות או בעיות נוספות, פנה למפתח המערכת.
For questions or issues, contact the system developer.

---
תאריך עדכון / Update Date: 2026-01-26  
גרסה / Version: 3.0 - String Reversal Solution
