#!/bin/bash

# תסריט הגדרת שרת TmsSystem לפריסה אוטומטית
# הרץ תסריט זה בשרת DigitalOcean שלך

set -e

echo "=========================================="
echo "   TmsSystem - הגדרת שרת לפריסה אוטומטית"
echo "=========================================="
echo ""

# צבעים
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# הגדרות
APP_DIR="/var/www/TmsSystem/TmsSystem"
BACKUP_DIR="/var/www/TmsSystem/backups"
SERVICE_NAME="tmssystem.service"
SERVICE_FILE="/etc/systemd/system/${SERVICE_NAME}"
DOTNET_VERSION="8.0"

echo -e "${YELLOW}שלב 1: בדיקת .NET Runtime${NC}"
if command -v dotnet &> /dev/null; then
    CURRENT_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET מותקן - גרסה: ${CURRENT_VERSION}${NC}"
else
    echo -e "${YELLOW}⚠ .NET לא מותקן. מתקין עכשיו...${NC}"
    
    # התקנת .NET Runtime - שימוש במקור רשמי של Microsoft
    # הורדה מ-Microsoft (לא מ-/tmp לביטחון)
    DOTNET_SCRIPT_DIR="$HOME/.dotnet-install"
    mkdir -p "$DOTNET_SCRIPT_DIR"
    
    # הורדת הסקריפט
    wget https://dot.net/v1/dotnet-install.sh -O "$DOTNET_SCRIPT_DIR/dotnet-install.sh"
    
    # בדיקת checksum (אופציונלי - אם יש)
    chmod +x "$DOTNET_SCRIPT_DIR/dotnet-install.sh"
    sudo "$DOTNET_SCRIPT_DIR/dotnet-install.sh" --channel ${DOTNET_VERSION} --runtime aspnetcore --install-dir /usr/share/dotnet
    
    # ניקוי
    rm -rf "$DOTNET_SCRIPT_DIR"
    
    # יצירת symlink
    if [ ! -f /usr/bin/dotnet ]; then
        sudo ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
    fi
    
    # וידוא התקנה
    if command -v dotnet &> /dev/null; then
        echo -e "${GREEN}✓ .NET הותקן בהצלחה${NC}"
    else
        echo -e "${RED}✗ שגיאה בהתקנת .NET${NC}"
        exit 1
    fi
fi

echo ""
echo -e "${YELLOW}שלב 2: יצירת תיקיות${NC}"

# יצירת תיקיות
sudo mkdir -p ${APP_DIR}
sudo mkdir -p ${BACKUP_DIR}

# הגדרת הרשאות
sudo chown -R www-data:www-data /var/www/TmsSystem
sudo chmod -R 755 /var/www/TmsSystem

echo -e "${GREEN}✓ תיקיות נוצרו בהצלחה${NC}"
echo "  - ${APP_DIR}"
echo "  - ${BACKUP_DIR}"

echo ""
echo -e "${YELLOW}שלב 3: יצירת Systemd Service${NC}"

# יצירת קובץ service
sudo tee ${SERVICE_FILE} > /dev/null <<EOF
[Unit]
Description=TMS System ASP.NET Core Application
After=network.target

[Service]
Type=notify
WorkingDirectory=${APP_DIR}
ExecStart=/usr/bin/dotnet ${APP_DIR}/TmsSystem.dll
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
EOF

# טעינת מחדש של systemd
sudo systemctl daemon-reload

# הפעלת השירות בהפעלה
sudo systemctl enable ${SERVICE_NAME}

echo -e "${GREEN}✓ Service נוצר והופעל${NC}"

echo ""
echo -e "${YELLOW}שלב 4: הגדרת Firewall (אופציונלי)${NC}"

# בדיקה אם ufw מותקן
if command -v ufw &> /dev/null; then
    echo "האם להגדיר חומת אש (UFW)? (y/n)"
    read -r -n 1 setup_firewall
    echo ""
    
    # בדיקת תקינות הקלט
    if [[ $setup_firewall =~ ^[Yy]$ ]]; then
        echo "מגדיר חומת אש..."
        sudo ufw allow 5000/tcp
        sudo ufw allow 80/tcp
        sudo ufw allow 443/tcp
        sudo ufw allow 22/tcp
        echo -e "${GREEN}✓ Firewall הוגדר${NC}"
    elif [[ $setup_firewall =~ ^[Nn]$ ]]; then
        echo -e "${YELLOW}מדלג על הגדרת Firewall${NC}"
    else
        echo -e "${YELLOW}⚠ קלט לא תקין, מדלג על הגדרת Firewall${NC}"
    fi
else
    echo -e "${YELLOW}⚠ UFW לא מותקן, מדלג...${NC}"
fi

echo ""
echo -e "${YELLOW}שלב 5: בדיקת הגדרות SSH${NC}"

if [ -d ~/.ssh ]; then
    echo -e "${GREEN}✓ תיקיית .ssh קיימת${NC}"
    
    if [ -f ~/.ssh/authorized_keys ]; then
        KEY_COUNT=$(grep -c "^ssh-" ~/.ssh/authorized_keys 2>/dev/null || echo 0)
        echo -e "${GREEN}✓ authorized_keys קיים עם ${KEY_COUNT} מפתחות${NC}"
    else
        echo -e "${YELLOW}⚠ authorized_keys לא קיים${NC}"
        echo "  יש להוסיף את המפתח הציבורי ל-~/.ssh/authorized_keys"
    fi
    
    # וידוא הרשאות
    chmod 700 ~/.ssh
    if [ -f ~/.ssh/authorized_keys ]; then
        chmod 600 ~/.ssh/authorized_keys
    fi
else
    echo -e "${YELLOW}⚠ תיקיית .ssh לא קיימת${NC}"
    mkdir -p ~/.ssh
    chmod 700 ~/.ssh
    echo "  תיקייה נוצרה. יש להוסיף מפתח ציבורי ל-~/.ssh/authorized_keys"
fi

echo ""
echo "=========================================="
echo -e "${GREEN}✓ ההגדרה הושלמה בהצלחה!${NC}"
echo "=========================================="
echo ""
echo "צעדים הבאים:"
echo ""
echo "1. ודא שהוספת את המפתח הציבורי ל-~/.ssh/authorized_keys"
echo ""
echo "2. הגדר Secrets ב-GitHub:"
echo "   - DIGITALOCEAN_HOST: $(hostname -I | awk '{print $1}')"
echo "   - DIGITALOCEAN_USERNAME: $(whoami)"
echo "   - DIGITALOCEAN_SSH_KEY: המפתח הפרטי"
echo ""
echo "3. דחוף שינויים ל-main branch והפריסה תתבצע אוטומטית!"
echo ""
echo "פקודות שימושיות:"
echo "  - סטטוס: sudo systemctl status ${SERVICE_NAME}"
echo "  - הפעלה: sudo systemctl start ${SERVICE_NAME}"
echo "  - עצירה: sudo systemctl stop ${SERVICE_NAME}"
echo "  - לוגים: sudo journalctl -u ${SERVICE_NAME} -f"
echo ""
