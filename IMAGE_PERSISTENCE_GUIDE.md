# מדריך שימור תמונות בעת עדכון השרת

## 🎯 מטרה
מסמך זה מסביר כיצד המערכת משמרת את התמונות שהועלו בעת עדכון האפליקציה בשרת.

## 📁 מיקום אחסון התמונות

התמונות מאוחסנות בתיקייה:
```
/var/www/TmsSystem/TmsSystem/wwwroot/uploads/
```

תיקייה זו מכילה:
- `uploads/trips/` - תמונות של ימי טיולים
- `uploads/gallery/` - תמונות מהגלריה

## 🔄 איך זה עובד?

### שלב 1: גיבוי אוטומטי
כאשר מתבצע deploy חדש, תהליך הפריסה:
1. **יוצר גיבוי זמני** של תיקיית `wwwroot/uploads` למיקום `/tmp/uploads_backup`
2. **מנקה** את כל קבצי האפליקציה הישנים
3. **מחלץ** את הגרסה החדשה
4. **משחזר** את תיקיית ה-uploads מהגיבוי
5. **מוחק** את הגיבוי הזמני

### שלב 2: שימור התמונות בגיט
התיקייה `wwwroot/` **אינה** מוגדרת ב-`.gitignore`, כלומר:
- תמונות שמועלות במהלך הפיתוח **יכללו** בגרסה
- ניתן לעקוב אחר שינויים בתמונות
- ניתן לשחזר תמונות מגרסאות קודמות

## ⚙️ הגדרות טכניות

### בקובץ Workflow (deploy-to-digitalocean.yml)

```yaml
# שמירת תיקיית uploads אם קיימת
if [ -d "$APP_DIR/wwwroot/uploads" ]; then
  echo "Backing up uploads folder..."
  sudo cp -r "$APP_DIR/wwwroot/uploads" /tmp/uploads_backup
fi

# ... ניקוי וחילוץ ...

# שחזור תיקיית uploads
if [ -d "/tmp/uploads_backup" ]; then
  echo "Restoring uploads folder..."
  sudo mkdir -p "$APP_DIR/wwwroot/uploads"
  sudo cp -r /tmp/uploads_backup/* "$APP_DIR/wwwroot/uploads/" 2>/dev/null || true
  sudo rm -rf /tmp/uploads_backup
fi
```

### ב-.gitignore
```
# wwwroot אינו מוגדר להתעלמות
# לכן כל התמונות נשמרות בגיט
```

## 📝 המלצות לניהול תמונות

### תמונות פיתוח (Development)
- העלה תמונות דרך ממשק המשתמש
- התמונות יישמרו ב-`wwwroot/uploads/`
- בצע commit וגם push כדי לשמור בגיט

### תמונות ייצור (Production)
- התמונות נשמרות אוטומטית בעת deploy
- גם אם תעשה deploy חדש, התמונות יישארו
- ניתן לגבות ידנית: `sudo tar -czf uploads_backup.tar.gz /var/www/TmsSystem/TmsSystem/wwwroot/uploads/`

## 🔧 פתרון בעיות

### אם תמונות נעלמו לאחר deploy
1. בדוק את הלוגים של ה-workflow ב-GitHub Actions
2. חפש את השורה: "Restoring uploads folder..."
3. אם לא מופיעה, בדוק שהתיקייה קיימת לפני ה-deploy

### גיבוי ידני
```bash
# התחבר לשרת
ssh user@server

# צור גיבוי
sudo tar -czf ~/uploads_backup_$(date +%Y%m%d).tar.gz /var/www/TmsSystem/TmsSystem/wwwroot/uploads/

# שחזור מגיבוי
sudo tar -xzf ~/uploads_backup_YYYYMMDD.tar.gz -C /
```

### שחזור מגרסה קודמת ב-Git
```bash
# מצא את הקומיט עם התמונות
git log --all --full-history -- "TmsSystem/wwwroot/uploads/*"

# שחזר תמונה ספציפית
git checkout <commit-hash> -- TmsSystem/wwwroot/uploads/trips/image.jpg
```

## ✅ בדיקת תקינות

אחרי כל deploy, בדוק:
1. **קיום התיקייה**: `ls -la /var/www/TmsSystem/TmsSystem/wwwroot/uploads/`
2. **מספר תמונות**: `find /var/www/TmsSystem/TmsSystem/wwwroot/uploads/ -type f | wc -l`
3. **הרשאות**: `ls -ld /var/www/TmsSystem/TmsSystem/wwwroot/uploads/`
   - צריך להיות: `drwxr-xr-x www-data www-data`

## 📊 סטטיסטיקות

התמונות הבאות נשמרות:
- תמונות ימי טיולים
- תמונות גלריה
- כל תמונה שהועלתה דרך המערכת

גודל משוער לניהול:
- תמונה ממוצעת: ~500KB-2MB
- סה"כ לטיול: ~10-50MB (בהתאם למספר ימים)
- מומלץ לבדוק נפח דיסק מדי פעם: `df -h /var/www`
