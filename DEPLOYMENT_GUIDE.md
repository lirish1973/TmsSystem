# מדריך הגדרת פריסה אוטומטית לשרת DigitalOcean

## 🎯 מטרה
מדריך זה מסביר כיצד להגדיר מערכת פריסה אוטומטית שתפרוס את האפליקציה לשרת DigitalOcean באופן אוטומטי בכל פעם שמבצעים push ל-branch הראשי (main).

## 📋 דרישות מוקדמות

### בשרת DigitalOcean
1. שרת Ubuntu פעיל (ubuntu-s-TmsServer-102025)
2. .NET 8.0 Runtime מותקן
3. גישת SSH עם מפתח SSH

### ב-GitHub
1. גישה למאגר (Repository) עם הרשאות ניהול
2. יכולת להוסיף Secrets למאגר

## 🔧 שלב 1: הגדרת השרת

### התקנת .NET 8.0 Runtime בשרת

התחבר לשרת דרך SSH והרץ את הפקודות הבאות:

```bash
# עדכון המערכת
sudo apt update && sudo apt upgrade -y

# התקנת .NET 8.0 Runtime
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/share/dotnet
sudo ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# וידוא ההתקנה
dotnet --version
```

### יצירת תיקיות האפליקציה

```bash
# יצירת תיקיות
sudo mkdir -p /var/www/TmsSystem/TmsSystem
sudo mkdir -p /var/www/TmsSystem/backups

# הגדרת הרשאות
sudo chown -R www-data:www-data /var/www/TmsSystem
sudo chmod -R 755 /var/www/TmsSystem
```

### יצירת קובץ Service ל-Systemd

צור קובץ service חדש:

```bash
sudo nano /etc/systemd/system/tmssystem.service
```

הוסף את התוכן הבא:

```ini
[Unit]
Description=TMS System ASP.NET Core Application
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/TmsSystem/TmsSystem
ExecStart=/usr/bin/dotnet /var/www/TmsSystem/TmsSystem/TmsSystem.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=tmssystem
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
```

הפעל את ה-Service:

```bash
# טעינת מחדש של systemd
sudo systemctl daemon-reload

# הפעלת השירות
sudo systemctl enable tmssystem.service

# ניתן להפעיל מאוחר יותר
# sudo systemctl start tmssystem.service
```

## 🔑 שלב 2: הכנת מפתח SSH

### יצירת מפתח SSH (אם אין לך)

בשרת ה-DigitalOcean או במחשב המקומי שלך:

```bash
# יצירת מפתח SSH חדש
ssh-keygen -t rsa -b 4096 -C "github-actions-deploy" -f ~/.ssh/github_deploy_key

# הצגת המפתח הפרטי (לשימוש ב-GitHub Secrets)
cat ~/.ssh/github_deploy_key

# הצגת המפתח הציבורי
cat ~/.ssh/github_deploy_key.pub
```

### הוספת המפתח הציבורי לשרת

בשרת DigitalOcean:

```bash
# הוספת המפתח הציבורי ל-authorized_keys
# החלף את התוכן בין המירכאות במפתח הציבורי שיצרת (תוכן הקובץ github_deploy_key.pub)
echo "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQ... github-actions-deploy-tmssystem" >> ~/.ssh/authorized_keys

# הגדרת הרשאות
chmod 600 ~/.ssh/authorized_keys
chmod 700 ~/.ssh
```

**טיפ:** אם רצת את `generate-ssh-keys.sh`, המפתח הציבורי הוצג במסך. העתק אותו כולל.

## 🔐 שלב 3: הגדרת GitHub Secrets

1. עבור למאגר ב-GitHub
2. לחץ על **Settings** (הגדרות)
3. בצד שמאל, לחץ על **Secrets and variables** > **Actions**
4. לחץ על **New repository secret**

הוסף את ה-Secrets הבאים:

### DIGITALOCEAN_HOST
- **Name**: `DIGITALOCEAN_HOST`
- **Value**: `64.225.67.19` (כתובת ה-IP של השרת שלך)

### DIGITALOCEAN_USERNAME
- **Name**: `DIGITALOCEAN_USERNAME`
- **Value**: `root` (או שם המשתמש שלך)

### DIGITALOCEAN_SSH_KEY
- **Name**: `DIGITALOCEAN_SSH_KEY`
- **Value**: תוכן המפתח הפרטי (כל התוכן של קובץ `github_deploy_key`)

**חשוב**: העתק את כל תוכן המפתח הפרטי כולל:
```
-----BEGIN OPENSSH PRIVATE KEY-----
...
-----END OPENSSH PRIVATE KEY-----
```

## 🚀 שלב 4: הפעלת הפריסה האוטומטית

### דחיפה ל-main branch

עכשיו בכל פעם שתדחוף שינויים ל-branch הראשי:

```bash
git add .
git commit -m "הודעת commit"
git push origin main
```

GitHub Actions יפעיל אוטומטית את תהליך הפריסה!

### הפעלה ידנית

אפשר גם להפעיל את הפריסה ידנית:

1. עבור ל-**Actions** במאגר GitHub
2. בחר את workflow **Deploy to DigitalOcean**
3. לחץ על **Run workflow**
4. בחר את ה-branch ולחץ על **Run workflow**

## 📊 שלב 5: מעקב אחר הפריסה

### בדיקת סטטוס ב-GitHub

1. עבור ל-**Actions** במאגר
2. בחר את הרצת ה-workflow האחרונה
3. לחץ על השלב כדי לראות logs מפורטים

### בדיקת סטטוס בשרת

התחבר לשרת ובדוק את סטטוס השירות:

```bash
# בדיקת סטטוס השירות
sudo systemctl status tmssystem.service

# צפייה ב-logs
sudo journalctl -u tmssystem.service -f

# בדיקת התיקייה
ls -la /var/www/TmsSystem/TmsSystem

# בדיקת גיבויים
ls -la /var/www/TmsSystem/backups
```

## 🔄 תהליך הפריסה האוטומטית

כאשר מבצעים push ל-main, המערכת:

1. ✅ בונה את האפליקציה עם .NET 8.0
2. ✅ יוצרת חבילת פריסה (deployment package)
3. ✅ מעלה את החבילה לשרת דרך SSH
4. ✅ יוצרת גיבוי של הגרסה הקיימת
5. ✅ עוצרת את השירות הפעיל
6. ✅ מחליפה את הקבצים הישנים בחדשים
7. ✅ מגדירה הרשאות
8. ✅ מפעילה מחדש את השירות
9. ✅ שומרת רק 5 גיבויים אחרונים

## 🛠️ פתרון בעיות

### הפריסה נכשלה

בדוק את ה-logs ב-GitHub Actions. שגיאות נפוצות:

1. **SSH Connection Failed**: בדוק שהמפתח הפרטי תקין ב-Secrets
2. **Permission Denied**: בדוק הרשאות בשרת
3. **Service Failed to Start**: בדוק logs בשרת עם `journalctl`

### השירות לא מתחיל

```bash
# בדיקת logs
sudo journalctl -u tmssystem.service -n 50 --no-pager

# בדיקת הגדרות
sudo systemctl cat tmssystem.service

# בדיקת קובץ ה-DLL
ls -la /var/www/TmsSystem/TmsSystem/TmsSystem.dll
```

### חזרה לגרסה קודמת (Rollback)

אם יש בעיה, אפשר לחזור לגיבוי:

```bash
# עצירת השירות
sudo systemctl stop tmssystem.service

# מציאת גיבוי אחרון
ls -lt /var/www/TmsSystem/backups/

# שחזור גיבוי
sudo rm -rf /var/www/TmsSystem/TmsSystem/*
sudo tar -xzf /var/www/TmsSystem/backups/backup_YYYYMMDD_HHMMSS.tar.gz -C /var/www/TmsSystem/TmsSystem

# הפעלה מחדש
sudo systemctl start tmssystem.service
```

## 📝 הערות נוספות

### אבטחה
- המפתח הפרטי ב-GitHub Secrets מוצפן ומאובטח
- אל תשתף את המפתח הפרטי בשום מקום אחר
- אפשר להגביל את המפתח רק לתיקיות מסוימות בשרת

### ביצועים
- המערכת שומרת רק 5 גיבויים אחרונים
- הגיבויים נמצאים ב-`/var/www/TmsSystem/backups`
- ניתן לשנות את מספר הגיבויים בקובץ workflow

### Nginx (אם יש)
אם יש לך Nginx כ-reverse proxy:

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## ✨ יתרונות המערכת החדשה

1. ✅ פריסה אוטומטית בכל push
2. ✅ גיבוי אוטומטי לפני כל פריסה
3. ✅ אין צורך במחיקה והעתקה ידנית
4. ✅ מעקב אחר היסטוריית פריסות
5. ✅ אפשרות לחזור לגרסה קודמת בקלות
6. ✅ תהליך בנייה עקבי ואמין

## 📞 תמיכה

אם יש בעיות או שאלות, בדוק את ה-logs:
- GitHub Actions logs
- Server logs: `sudo journalctl -u tmssystem.service -f`
- Application logs בתיקיית האפליקציה

---

**זה הכל! המערכת שלך מוכנה לפריסה אוטומטית! 🎉**
