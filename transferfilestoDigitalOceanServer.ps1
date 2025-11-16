# הגדרות חיבור
$localPath = "F:\VBprojects\TMS\TmsSystem\TmsSystem"
$remoteUser = "root"
$remoteHost = "64.225.67.19"
$remotePath = "/var/www/TmsSystem/TmsSystem"

Write-Host "================================" -ForegroundColor Cyan
Write-Host "סקריפט העתקת קבצים לשרת TMS" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "מקור: $localPath" -ForegroundColor Yellow
Write-Host "יעד: $remoteUser@$remoteHost:$remotePath" -ForegroundColor Yellow
Write-Host ""

# בדיקה אם התיקייה קיימת וספירת קבצים
if (-not (Test-Path $localPath)) {
    Write-Error "התיקייה המקומית לא נמצאה: $localPath"
    exit 1
}

$fileCount = (Get-ChildItem -Path $localPath -Recurse -File).Count
$folderCount = (Get-ChildItem -Path $localPath -Recurse -Directory).Count

Write-Host "נמצאו: $fileCount קבצים ו-$folderCount תיקיות" -ForegroundColor Cyan
Write-Host ""

# אישור המשתמש
$confirm = Read-Host "להמשיך בהעתקה? (Y/N)"
if ($confirm -ne 'Y' -and $confirm -ne 'y') {
    Write-Host "הפעולה בוטלה על ידי המשתמש" -ForegroundColor Yellow
    exit 0
}

# בדיקת חיבור
Write-Host ""
Write-Host "בודק חיבור לשרת $remoteHost..." -ForegroundColor Yellow
$pingResult = Test-Connection -ComputerName $remoteHost -Count 2 -Quiet

if ($pingResult) {
    Write-Host "✓ השרת זמין" -ForegroundColor Green
} else {
    Write-Warning "⚠ לא ניתן להגיע לשרת"
}

Write-Host ""
Write-Host "מתחיל העתקה..." -ForegroundColor Green
Write-Host "הערה: ייתכן שתתבקש להזין סיסמה" -ForegroundColor Yellow
Write-Host ""

try {
    # העתקה עם SCP
    scp -r -p "$localPath\*" "${remoteUser}@${remoteHost}:${remotePath}/"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "================================" -ForegroundColor Green
        Write-Host "✓ ההעתקה הושלמה בהצלחה!" -ForegroundColor Green
        Write-Host "================================" -ForegroundColor Green
        
        # רישום לוג
        $logEntry = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') - העתקה מוצלחת ל-$remoteHost"
        Add-Content -Path "$PSScriptRoot\copy-log.txt" -Value $logEntry
    } else {
        throw "SCP נכשל עם קוד שגיאה: $LASTEXITCODE"
    }
} catch {
    Write-Host ""
    Write-Host "================================" -ForegroundColor Red
    Write-Host "✗ ההעתקה נכשלה" -ForegroundColor Red
    Write-Host "================================" -ForegroundColor Red
    Write-Error $_.Exception.Message
    
    Write-Host ""
    Write-Host "פתרון בעיות:" -ForegroundColor Yellow
    Write-Host "1. בדוק אם OpenSSH מותקן:" -ForegroundColor White
    Write-Host "   Get-WindowsCapability -Online | Where-Object Name -like 'OpenSSH.Client*'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. להתקנת OpenSSH:" -ForegroundColor White
    Write-Host "   Add-WindowsCapability -Online -Name OpenSSH.Client~~~~0.0.1.0" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. בדוק חיבור SSH ידנית:" -ForegroundColor White
    Write-Host "   ssh $remoteUser@$remoteHost" -ForegroundColor Gray
}