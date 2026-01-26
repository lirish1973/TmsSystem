# תיקון טקסט עברי הפוך ב-PDF / Hebrew PDF Fix

## הבעיה / Problem
טקסט עברי הופיע הפוך (במראה ראי) בקבצי PDF שנוצרו על ידי המערכת.
Hebrew text was appearing reversed (mirrored) in generated PDF files.

## הפתרון / Solution  
**Updated 2026-01-26 (v3.1):** החלפנו את גישת סמני BiDi בהיפוך ידני של מחרוזות עבריות **שורה-אחר-שורה** לפני יצירת ה-PDF. זה משמר את סדר השורות הנכון בעוד שמתקן את סדר התווים.

We replaced the BiDi marker approach with manual Hebrew string reversal **line-by-line** before PDF generation. This preserves correct line order while fixing character order.

### הסיבה לשינוי / Why the Change
הגישה הקודמת עם סמני BiDi Unicode (RLE/PDF) לא עבדה כהלכה עם iText7. ספריית iText7 הופכת את הטקסט העברי בזמן יצירת ה-PDF, ללא קשר לסמני BiDi.

**עדכון חשוב:** הגרסה הקודמת שהפכה את כל הטקסט כמחרוזת אחת גרמה לסדר שורות הפוך בתיאורים רב-שורתיים. הגרסה המעודכנת מפצלת את הטקסט לשורות, מהפכת כל שורה בנפרד, ולאחר מכן מחברת אותן בחזרה - זה משמר את סדר הפסקאות.

The previous BiDi marker approach didn't work properly with iText7. The iText7 library reverses Hebrew text during PDF generation, regardless of BiDi markers.

**Important Update:** The previous version that reversed entire text as one string caused reversed line order in multi-line descriptions. The updated version splits text into lines, reverses each line separately, then rejoins them - this preserves paragraph order.

### איך זה עובד / How It Works
1. **זיהוי טקסט עברי** - הפונקציה `ReverseHebrewText()` בודקת אם יש תווים עבריים (U+0590-U+05FF)
2. **קידוד HTML** - הטקסט עובר קידוד HTML לבטיחות (לפני ההיפוך)
3. **פיצול לשורות** - הטקסט מפוצל לשורות נפרדות כדי לשמר את סדר השורות
4. **היפוך שורה-אחר-שורה** - כל שורה מתהפכת בנפרד (לא כל המחרוזת)
5. **חיבור מחדש** - השורות מחוברות בחזרה עם שורות חדשות
6. **iText7 מהפך שוב** - כשiText7 יוצר את ה-PDF, הוא מהפך כל שורה שוב, וכך היא מגיעה למצב הנכון

1. **Hebrew detection** - The `ReverseHebrewText()` function checks for Hebrew characters (U+0590-U+05FF)
2. **HTML encoding** - Text is HTML-encoded for safety (before reversal)
3. **Split into lines** - Text is split into separate lines to preserve line order
4. **Line-by-line reversal** - Each line is reversed individually (not the entire string)
5. **Rejoin** - Lines are joined back together with newlines
6. **iText7 reverses again** - When iText7 generates the PDF, it reverses each line again, resulting in correct display

## שינויים שבוצעו / Changes Made

### 1. פונקציה מעודכנת / Updated Function
```csharp
private string ReverseHebrewText(string? text)
{
    if (string.IsNullOrEmpty(text))
        return string.Empty;
    
    // Check if text contains Hebrew characters
    bool hasHebrew = text.Any(c => c >= 0x0590 && c <= 0x05FF);
    
    if (!hasHebrew)
        return text;
    
    // Split text into lines to preserve line order
    var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    var reversedLines = new List<string>();
    
    foreach (var line in lines)
    {
        if (string.IsNullOrEmpty(line))
        {
            // Preserve empty lines
            reversedLines.Add(line);
        }
        else
        {
            // Reverse each line individually to compensate for iText7's reversal
            char[] charArray = line.ToCharArray();
            Array.Reverse(charArray);
            reversedLines.Add(new string(charArray));
        }
    }
    
    // Rejoin lines with line breaks
    return string.Join("\n", reversedLines);
}
```

### 2. עדכון פונקציית עזר / Updated Helper Function
```csharp
private string EncodeAndReverseRtl(string? text, string defaultValue = "לא צוין")
{
    var textToUse = text ?? defaultValue;
    // First HTML encode for safety, then reverse if Hebrew
    var encoded = HttpUtility.HtmlEncode(textToUse);
    return ReverseHebrewText(encoded);
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

## דוגמאות / Examples

### דוגמה 1: טקסט חד-שורתי / Single-line Text

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

### דוגמה 2: טקסט רב-שורתי / Multi-line Text

**Input (before reversal):**
```
לאחר ארוחת הבוקר נצא בנסיעה לצפון האי
(Nungwi). זהו כפר דייגים מסורתי
מאבן האלמוג המקומית.
```

**After ReverseHebrewText (line-by-line):**
```
יאה ןופצל העיסנב אצנ רקובה תחורא רחאל
יתרוסמ םיגייד רפכ והז .)iwgnuN(
.תימוקמה גומלאה ןבאמ
```

**After iText7 processing (in PDF):**
```
לאחר ארוחת הבוקר נצא בנסיעה לצפון האי
(Nungwi). זהו כפר דייגים מסורתי
מאבן האלמוג המקומית.
```
✅ **סדר שורות נכון! / Correct line order!**

### מה שינה העדכון / What the Update Changed

**לפני העדכון (v3.0):** הפיכת כל הטקסט כמחרוזת אחת → **סדר שורות הפוך** ❌  
**אחרי העדכון (v3.1):** הפיכת כל שורה בנפרד → **סדר שורות נכון** ✅

**Before Update (v3.0):** Reversing entire text as one string → **Reversed line order** ❌  
**After Update (v3.1):** Reversing each line separately → **Correct line order** ✅

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
גרסה / Version: 3.1 - Line-by-Line String Reversal Solution
