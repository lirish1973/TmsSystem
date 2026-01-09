# מדריך התחלה מהיר - פריסה אוטומטית

## 🚀 התחלה מהירה ב-3 שלבים

### שלב 1: הגדרת GitHub Secrets

עבור ל-**Settings** > **Secrets and variables** > **Actions** במאגר GitHub והוסף:

| שם Secret | ערך | הסבר |
|-----------|-----|------|
| `DIGITALOCEAN_HOST` | `64.225.67.19` | כתובת IP של השרת |
| `DIGITALOCEAN_USERNAME` | `root` | שם משתמש SSH |
| `DIGITALOCEAN_SSH_KEY` | `-----BEGIN OPENSSH PRIVATE KEY-----...` | מפתח SSH פרטי |

### שלב 2: הגדרת השרת (פעם אחת)

התחבר לשרת והרץ:

```bash
# יצירת תיקיות
sudo mkdir -p /var/www/TmsSystem/TmsSystem
sudo mkdir -p /var/www/TmsSystem/backups

# יצירת service
sudo tee /etc/systemd/system/tmssystem.service > /dev/null <<EOF
[Unit]
Description=TMS System
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/TmsSystem/TmsSystem
ExecStart=/usr/bin/dotnet /var/www/TmsSystem/TmsSystem/TmsSystem.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
EOF

# הפעלת service
sudo systemctl daemon-reload
sudo systemctl enable tmssystem.service
```

### שלב 3: הפעלת פריסה

זהו! עכשיו כל פעם שתעשה:

```bash
git push origin main
```

המערכת תפרוס אוטומטית לשרת! 🎉

---

## 📋 בדיקת סטטוס

### ב-GitHub
עבור ל-**Actions** כדי לראות את התקדמות הפריסה

### בשרת
```bash
# סטטוס השירות
sudo systemctl status tmssystem.service

# צפייה ב-logs
sudo journalctl -u tmssystem.service -f
```

---

## 🔄 חזרה לגרסה קודמת

```bash
sudo systemctl stop tmssystem.service
sudo rm -rf /var/www/TmsSystem/TmsSystem/*
sudo tar -xzf /var/www/TmsSystem/backups/backup_*.tar.gz -C /var/www/TmsSystem/TmsSystem
sudo systemctl start tmssystem.service
```

---

📖 **למדריך מפורט**: ראה [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)
