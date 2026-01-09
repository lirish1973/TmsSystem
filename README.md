# TmsSystem

## 🚀 פריסה אוטומטית לשרת DigitalOcean

מערכת זו כוללת פריסה אוטומטית עם GitHub Actions. בכל push ל-main branch, האפליקציה תיבנה ותפרוס אוטומטית לשרת.

### ⚡ התחלה מהירה

1. **הגדרת השרת** - הרץ את הסקריפט בשרת:
   ```bash
   curl -o setup-server.sh https://raw.githubusercontent.com/lirish1973/TmsSystem/main/setup-server.sh
   chmod +x setup-server.sh
   sudo ./setup-server.sh
   ```

2. **הגדרת GitHub Secrets** - הוסף את הערכים הבאים ב-Settings > Secrets:
   - `DIGITALOCEAN_HOST` - כתובת IP של השרת (64.225.67.19)
   - `DIGITALOCEAN_USERNAME` - שם משתמש SSH (root)
   - `DIGITALOCEAN_SSH_KEY` - מפתח SSH פרטי

3. **פריסה** - דחוף שינויים ל-main:
   ```bash
   git push origin main
   ```

### 📚 תיעוד מלא

| מדריך | תיאור |
|-------|-------|
| [סיכום מערכת הפריסה](AUTO_DEPLOYMENT_SUMMARY.md) | מבט על ומה נוצר |
| [התחלה מהירה](QUICK_SETUP.md) | הקמה ב-3 שלבים |
| [מדריך מפורט](DEPLOYMENT_GUIDE.md) | הסבר מלא עם כל הפרטים |
| [תרשים זרימה](DEPLOYMENT_FLOW.md) | ויזואליזציה של התהליך |
| [פתרון בעיות](TROUBLESHOOTING.md) | פתרונות לבעיות נפוצות |

### 🛠️ סקריפטים עזר

- `setup-server.sh` - הגדרת שרת אוטומטית
- `generate-ssh-keys.sh` - יצירת מפתחות SSH

---