// כרטיסיית הצלחה לשליחת מייל
class EmailSuccessCard {
    constructor() {
        this.overlay = null;
        this.autoCloseTimer = null;
    }

    show(data) {
        // הסרת כרטיסיה קיימת אם יש
        this.close();

        // יצירת הכרטיסיה
        this.createCard(data);

        // הצגת הכרטיסיה עם אנימציה
        document.body.appendChild(this.overlay);

        setTimeout(() => {
            this.overlay.classList.add('show');
            this.createParticles();
        }, 10);

        // הגדרת סגירה אוטומטית
        this.autoCloseTimer = setTimeout(() => {
            this.close();
        }, 10000);

        // הוספת מאזינים לסגירה
        this.addEventListeners();
    }

    createCard(data) {
        this.overlay = document.createElement('div');
        this.overlay.className = 'email-success-overlay';

        const card = document.createElement('div');
        card.className = 'email-success-card';

        card.innerHTML = `
            <div class="particles"></div>
            <div class="success-icon">
                <i class="fas fa-check-circle"></i>
            </div>
            <div class="success-title">המייל נשלח בהצלחה! 🎉</div>
            
            <div class="email-details">
                <div class="detail-row">
                    <span class="detail-label">
                        <i class="fas fa-envelope detail-icon"></i>
                        נשלח אל:
                    </span>
                    <span class="detail-value" dir="ltr">${data.sentTo}</span>
                </div>
                <div class="detail-row">
                    <span class="detail-label">
                        <i class="fas fa-tag detail-icon"></i>
                        נושא:
                    </span>
                    <span class="detail-value">${data.subject}</span>
                </div>
                <div class="detail-row">
                    <span class="detail-label">
                        <i class="fas fa-clock detail-icon"></i>
                        זמן שליחה:
                    </span>
                    <span class="detail-value">${data.sentAt}</span>
                </div>
                <div class="detail-row">
                    <span class="detail-label">
                        <i class="fas fa-server detail-icon"></i>
                        ספק שליחה:
                    </span>
                    <span class="detail-value">${data.provider}</span>
                </div>
                <div class="detail-row">
                    <span class="detail-label">
                        <i class="fas fa-hashtag detail-icon"></i>
                        הצעה מספר:
                    </span>
                    <span class="detail-value">#${data.offerId}</span>
                </div>
                ${data.customerName ? `
                <div class="detail-row">
                    <span class="detail-label">
                        <i class="fas fa-user detail-icon"></i>
                        לקוח:
                    </span>
                    <span class="detail-value">${data.customerName}</span>
                </div>
                ` : ''}
                ${data.messageId ? `
                <div class="detail-row">
                    <span class="detail-label">
                        <i class="fas fa-fingerprint detail-icon"></i>
                        מזהה הודעה:
                    </span>
                    <span class="detail-value" style="font-size: 12px;">${data.messageId}</span>
                </div>
                ` : ''}
            </div>
            
            <div class="success-actions">
                <button class="close-btn" onclick="emailSuccessCard.close()">
                    <i class="fas fa-check"></i> סגור
                </button>
            </div>
        `;

        this.overlay.appendChild(card);
    }

    createParticles() {
        const particlesContainer = this.overlay.querySelector('.particles');
        const colors = ['#28a745', '#20c997', '#17a2b8', '#6f42c1'];

        for (let i = 0; i < 15; i++) {
            setTimeout(() => {
                const particle = document.createElement('div');
                particle.className = 'particle';
                particle.style.left = Math.random() * 100 + '%';
                particle.style.background = colors[Math.floor(Math.random() * colors.length)];
                particle.style.width = particle.style.height = (Math.random() * 8 + 4) + 'px';
                particle.style.animationDuration = (Math.random() * 3 + 2) + 's';
                particle.style.animationDelay = Math.random() * 2 + 's';

                particlesContainer.appendChild(particle);

                setTimeout(() => {
                    if (particle.parentNode) {
                        particle.remove();
                    }
                }, 5000);
            }, i * 200);
        }
    }

    addEventListeners() {
        // סגירה בלחיצה על הרקע
        this.overlay.addEventListener('click', (e) => {
            if (e.target === this.overlay) {
                this.close();
            }
        });

        // סגירה במקש ESC
        document.addEventListener('keydown', this.handleKeyPress.bind(this));
    }

    handleKeyPress(e) {
        if (e.key === 'Escape' && this.overlay) {
            this.close();
        }
    }

    close() {
        if (this.overlay) {
            const card = this.overlay.querySelector('.email-success-card');
            this.overlay.classList.remove('show');
            card.style.transform = 'scale(0.7) translateY(50px)';

            setTimeout(() => {
                if (this.overlay && this.overlay.parentNode) {
                    this.overlay.remove();
                }
                this.overlay = null;
            }, 300);
        }

        if (this.autoCloseTimer) {
            clearTimeout(this.autoCloseTimer);
            this.autoCloseTimer = null;
        }

        document.removeEventListener('keydown', this.handleKeyPress);
    }
}

// יצירת instance גלובלי
const emailSuccessCard = new EmailSuccessCard();

// פונקציית שליחת מייל מעודכנת
function sendOfferEmail(offerId) {
    const btn = event.target;
    const originalText = btn.innerHTML;

    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> שולח...';

    // מצא את שדה המייל
    const emailInput = document.querySelector(`#email-${offerId}`) ||
        document.querySelector(`input[name="email"]`) ||
        document.querySelector('input[type="email"]');

    const email = emailInput ? emailInput.value : '';

    if (!email) {
        alert('יש להזין כתובת מייל');
        btn.innerHTML = originalText;
        btn.disabled = false;
        return;
    }

    fetch(`/Offers/SendOfferEmail`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `id=${offerId}&email=${encodeURIComponent(email)}`
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                emailSuccessCard.show(data);
                btn.innerHTML = '<i class="fas fa-check"></i> נשלח!';
                btn.classList.remove('btn-primary');
                btn.classList.add('btn-success');
            } else {
                btn.innerHTML = '<i class="fas fa-exclamation-triangle"></i> שגיאה';
                btn.classList.add('btn-danger');
                alert(data.message || 'שגיאה בשליחה');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            btn.innerHTML = '<i class="fas fa-exclamation-triangle"></i> שגיאה';
            btn.classList.add('btn-danger');
            alert('שגיאה בשליחה');
        })
        .finally(() => {
            setTimeout(() => {
                btn.innerHTML = originalText;
                btn.disabled = false;
                btn.classList.remove('btn-success', 'btn-danger');
                btn.classList.add('btn-primary');
            }, 3000);
        });
}