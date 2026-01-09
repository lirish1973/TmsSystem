#!/bin/bash

# תסריט ליצירת מפתחות SSH לפריסה אוטומטית
# הרץ תסריט זה במחשב המקומי או בשרת

echo "=========================================="
echo "   יצירת מפתחות SSH לפריסה אוטומטית"
echo "=========================================="
echo ""

# הגדרות
KEY_NAME="github_deploy_key"
KEY_PATH="$HOME/.ssh/${KEY_NAME}"
KEY_COMMENT="github-actions-deploy-tmssystem"

# בדיקה אם תיקיית .ssh קיימת
if [ ! -d "$HOME/.ssh" ]; then
    echo "יוצר תיקיית .ssh..."
    mkdir -p "$HOME/.ssh"
    chmod 700 "$HOME/.ssh"
fi

# בדיקה אם המפתח כבר קיים
if [ -f "$KEY_PATH" ]; then
    echo "⚠️  מפתח כבר קיים ב-$KEY_PATH"
    echo ""
    read -p "האם למחוק ולייצר מחדש? (y/n): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "פעולה בוטלה."
        exit 0
    fi
    rm -f "$KEY_PATH" "$KEY_PATH.pub"
fi

echo ""
echo "יוצר מפתח SSH חדש..."
echo ""

# יצירת מפתח
ssh-keygen -t rsa -b 4096 -C "$KEY_COMMENT" -f "$KEY_PATH" -N ""

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "✓ המפתח נוצר בהצלחה!"
    echo "=========================================="
    echo ""
    
    echo "📁 מיקום הקבצים:"
    echo "   מפתח פרטי: $KEY_PATH"
    echo "   מפתח ציבורי: $KEY_PATH.pub"
    echo ""
    
    echo "=========================================="
    echo "🔑 מפתח ציבורי (להוספה לשרת):"
    echo "=========================================="
    cat "$KEY_PATH.pub"
    echo ""
    
    echo "📋 צעדים הבאים:"
    echo ""
    echo "1️⃣  העתק את המפתח הציבורי לשרת:"
    echo ""
    echo "   ssh root@64.225.67.19 'mkdir -p ~/.ssh && chmod 700 ~/.ssh'"
    echo "   cat $KEY_PATH.pub | ssh root@64.225.67.19 'cat >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys'"
    echo ""
    echo "   או ידנית:"
    echo "   ssh root@64.225.67.19"
    echo "   echo \"$(cat $KEY_PATH.pub)\" >> ~/.ssh/authorized_keys"
    echo "   chmod 600 ~/.ssh/authorized_keys"
    echo ""
    
    echo "2️⃣  בדוק את החיבור:"
    echo ""
    echo "   ssh -i $KEY_PATH root@64.225.67.19"
    echo ""
    
    echo "3️⃣  הוסף את המפתח הפרטי ל-GitHub Secrets:"
    echo ""
    echo "   עבור ל-GitHub → Settings → Secrets and variables → Actions"
    echo "   לחץ 'New repository secret'"
    echo ""
    echo "   שם: DIGITALOCEAN_SSH_KEY"
    echo "   ערך: העתק את כל התוכן של המפתח הפרטי:"
    echo ""
    echo "   =========================================="
    echo "   🔐 מפתח פרטי (להעתקה ל-GitHub Secret):"
    echo "   =========================================="
    cat "$KEY_PATH"
    echo ""
    echo "   =========================================="
    echo ""
    
    echo "4️⃣  הוסף את שאר ה-Secrets:"
    echo ""
    echo "   DIGITALOCEAN_HOST:"
    echo "   64.225.67.19"
    echo ""
    echo "   DIGITALOCEAN_USERNAME:"
    echo "   root"
    echo ""
    
    echo "✅ סיימת! עכשיו תוכל לדחוף ל-main והפריסה תתבצע אוטומטית!"
    echo ""
    
    echo "⚠️  אזהרת אבטחה:"
    echo "   המפתח הפרטי נמצא ב-$KEY_PATH"
    echo "   אל תשתף אותו עם אף אחד ואל תעלה אותו ל-Git!"
    echo "   לאחר הוספה ל-GitHub Secrets, שמור את המפתח במקום מאובטח."
    echo ""
    
else
    echo ""
    echo "❌ שגיאה ביצירת המפתח"
    exit 1
fi
