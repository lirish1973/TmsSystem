# מדריך פתרון בעיות - פריסה אוטומטית

## 🔍 בדיקות ראשוניות

### בדיקת סטטוס GitHub Actions

1. עבור ל-**Actions** במאגר GitHub
2. בחר את הרצת ה-workflow האחרונה
3. בדוק את הלוגים של כל שלב

### בדיקת סטטוס בשרת

```bash
# חיבור לשרת
ssh root@64.225.67.19

# בדיקת סטטוס השירות
sudo systemctl status tmssystem.service

# צפייה בלוגים חיים
sudo journalctl -u tmssystem.service -f

# צפייה ב-50 השורות האחרונות
sudo journalctl -u tmssystem.service -n 50 --no-pager
```

---

## ❌ שגיאות נפוצות ופתרונות

### 1. שגיאת SSH Connection Failed

**תסמינים:**
```
Error: ssh: connect to host 64.225.67.19 port 22: Connection refused
```

**פתרונות:**

#### א. בדיקת חיבור לשרת
```bash
# בדיקת ping
ping -c 4 64.225.67.19

# בדיקת חיבור SSH
ssh -v root@64.225.67.19
```

#### ב. בדיקת המפתח הפרטי ב-GitHub Secrets

1. עבור ל-**Settings** > **Secrets and variables** > **Actions**
2. ודא ש-`DIGITALOCEAN_SSH_KEY` קיים
3. ודא שהמפתח כולל את כל השורות:
   ```
   -----BEGIN OPENSSH PRIVATE KEY-----
   ...כל התוכן...
   -----END OPENSSH PRIVATE KEY-----
   ```

#### ג. בדיקת המפתח הציבורי בשרת

```bash
# התחבר לשרת
ssh root@64.225.67.19

# בדוק את הקובץ
cat ~/.ssh/authorized_keys

# ודא הרשאות נכונות
chmod 700 ~/.ssh
chmod 600 ~/.ssh/authorized_keys
```

---

### 2. שגיאת Permission Denied

**תסמינים:**
```
Permission denied (publickey)
```

**פתרונות:**

#### א. בדיקת שם המשתמש
ודא ב-GitHub Secrets ש-`DIGITALOCEAN_USERNAME` הוא `root` או המשתמש הנכון

#### ב. בדיקת הרשאות בשרת
```bash
# הרשאות תיקיית .ssh
ls -la ~/.ssh

# צריך להיות:
# drwx------ (700) .ssh
# -rw------- (600) authorized_keys
```

#### ג. בדיקת לוג SSH בשרת
```bash
sudo tail -f /var/log/auth.log
# או
sudo journalctl -u ssh -f
```

---

### 3. שגיאת Build Failed

**תסמינים:**
```
Error: Build FAILED
```

**פתרונות:**

#### א. בדיקת תלויות
```bash
# במחשב המקומי
cd TmsSystem
dotnet restore
dotnet build
```

#### ב. בדיקת גרסת .NET
ודא שבשרת מותקנת .NET 8.0:
```bash
dotnet --version
```

אם לא מותקן:
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/share/dotnet
```

---

### 4. השירות לא מתחיל

**תסמינים:**
```
Failed to start TmsSystem service
```

**פתרונות:**

#### א. בדיקת לוגים
```bash
sudo journalctl -u tmssystem.service -xe
```

#### ב. בדיקת קובץ ה-DLL
```bash
ls -la /var/www/TmsSystem/TmsSystem/TmsSystem.dll

# צריך להיות קיים עם הרשאות +x
```

#### ג. בדיקת קובץ ה-Service
```bash
sudo systemctl cat tmssystem.service
```

ודא שהקובץ נכון:
```ini
[Service]
WorkingDirectory=/var/www/TmsSystem/TmsSystem
ExecStart=/usr/bin/dotnet /var/www/TmsSystem/TmsSystem/TmsSystem.dll
```

#### ד. בדיקת פורט
```bash
# בדוק אם פורט 5000 תפוס
sudo netstat -tlnp | grep 5000

# או
sudo ss -tlnp | grep 5000
```

אם הפורט תפוס:
```bash
# מצא את התהליך
sudo lsof -i :5000

# עצור אותו
sudo kill -9 <PID>
```

#### ה. הפעלה ידנית לבדיקה
```bash
cd /var/www/TmsSystem/TmsSystem
dotnet TmsSystem.dll
```

בדוק את השגיאות שמופיעות.

---

### 5. שגיאת Database Connection

**תסמינים:**
```
Unable to connect to MySQL server
```

**פתרונות:**

#### א. בדיקת ה-Connection String

בשרת, בדוק את `appsettings.json`:
```bash
cat /var/www/TmsSystem/TmsSystem/appsettings.json | grep -A 3 ConnectionStrings
```

#### ב. בדיקת MySQL
```bash
# בדוק אם MySQL פעיל
sudo systemctl status mysql

# נסה להתחבר
mysql -u root -p

# בדוק אם המסד נתונים קיים
mysql -u root -p -e "SHOW DATABASES;"
```

#### ג. הגדרת משתמש MySQL
```sql
CREATE DATABASE IF NOT EXISTS tmssystem;
CREATE USER 'tmsuser'@'localhost' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON tmssystem.* TO 'tmsuser'@'localhost';
FLUSH PRIVILEGES;
```

---

### 6. הפריסה הצליחה אבל האתר לא עובד

**פתרונות:**

#### א. בדיקת הפורט
```bash
# בדוק אם האפליקציה מאזינה
curl http://localhost:5000
```

#### ב. בדיקת Firewall
```bash
# בדוק את החומת אש
sudo ufw status

# אם צריך לפתוח פורט
sudo ufw allow 5000/tcp
sudo ufw allow 80/tcp
```

#### ג. הגדרת Nginx (אם משתמשים)
```bash
# בדוק את Nginx
sudo systemctl status nginx

# קובץ הגדרה לדוגמה
sudo nano /etc/nginx/sites-available/tmssystem
```

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
    }
}
```

---

### 7. הגיבויים מתמלאים דיסק

**פתרונות:**

#### א. בדיקת שימוש בדיסק
```bash
df -h
du -sh /var/www/TmsSystem/backups/*
```

#### ב. מחיקת גיבויים ישנים
```bash
# שמירת 3 גיבויים אחרונים בלבד
cd /var/www/TmsSystem/backups
ls -t backup_*.tar.gz | tail -n +4 | xargs sudo rm -v
```

#### ג. שינוי כמות הגיבויים
ערוך את workflow ב-`.github/workflows/deploy-to-digitalocean.yml`:
```bash
# שינוי מ-5 ל-3
ls -t backup_*.tar.gz | tail -n +4 | xargs -r sudo rm
```

---

### 8. רוצה לחזור לגרסה קודמת (Rollback)

**צעדים:**

```bash
# 1. התחבר לשרת
ssh root@64.225.67.19

# 2. עצור את השירות
sudo systemctl stop tmssystem.service

# 3. רשום את הרשימה של גיבויים
ls -lt /var/www/TmsSystem/backups/

# 4. בחר גיבוי לשחזור
BACKUP_FILE="backup_20260109_120000.tar.gz"

# 5. נקה את התיקייה הנוכחית
sudo rm -rf /var/www/TmsSystem/TmsSystem/*

# 6. שחזר את הגיבוי
sudo tar -xzf /var/www/TmsSystem/backups/$BACKUP_FILE -C /var/www/TmsSystem/TmsSystem

# 7. הגדר הרשאות
sudo chown -R www-data:www-data /var/www/TmsSystem/TmsSystem
sudo chmod -R 755 /var/www/TmsSystem/TmsSystem

# 8. הפעל מחדש את השירות
sudo systemctl start tmssystem.service

# 9. בדוק סטטוס
sudo systemctl status tmssystem.service
```

---

### 9. בעיות עם GitHub Secrets

**בדיקה:**

#### א. ודא שכל ה-Secrets קיימים
```
✓ DIGITALOCEAN_HOST
✓ DIGITALOCEAN_USERNAME
✓ DIGITALOCEAN_SSH_KEY
```

#### ב. פורמט נכון
- `DIGITALOCEAN_HOST`: רק IP, בלי http:// או https://
  ```
  64.225.67.19
  ```
  
- `DIGITALOCEAN_USERNAME`: רק שם המשתמש
  ```
  root
  ```
  
- `DIGITALOCEAN_SSH_KEY`: כל המפתח הפרטי
  ```
  -----BEGIN OPENSSH PRIVATE KEY-----
  b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9...
  ...כל השורות...
  -----END OPENSSH PRIVATE KEY-----
  ```

---

## 🔧 פקודות שימושיות

### ניטור השירות
```bash
# סטטוס
sudo systemctl status tmssystem.service

# הפעלה
sudo systemctl start tmssystem.service

# עצירה
sudo systemctl stop tmssystem.service

# הפעלה מחדש
sudo systemctl restart tmssystem.service

# לוגים חיים
sudo journalctl -u tmssystem.service -f

# לוגים של היום
sudo journalctl -u tmssystem.service --since today
```

### בדיקת קבצים
```bash
# רשימת קבצים באפליקציה
ls -lah /var/www/TmsSystem/TmsSystem/

# גודל התיקייה
du -sh /var/www/TmsSystem/TmsSystem/

# בדיקת גרסת קובץ
stat /var/www/TmsSystem/TmsSystem/TmsSystem.dll
```

### בדיקת רשת
```bash
# בדיקת חיבור
curl http://localhost:5000

# בדיקת פורטים פתוחים
sudo netstat -tlnp

# בדיקת חומת אש
sudo ufw status
```

---

## 📞 תמיכה נוספת

אם אף אחד מהפתרונות לא עזר:

1. **אסוף מידע:**
   ```bash
   # לוגים של השירות
   sudo journalctl -u tmssystem.service -n 100 --no-pager > service.log
   
   # לוגים של GitHub Actions
   # שמור מ-Actions > הרצה אחרונה > כל השלבים
   ```

2. **בדוק גרסאות:**
   ```bash
   dotnet --version
   uname -a
   ```

3. **בדוק נתונים:**
   ```bash
   df -h
   free -h
   ```

---

💡 **טיפ:** שמור את הפקודות האלה בקובץ נפרד לגישה מהירה!
