// פונקציה לשליחת הצעת מחיר לטיול במייל
async function sendTripOfferEmail(tripOfferId) {
    console.log('🚀 sendTripOfferEmail called with ID:', tripOfferId);

    if (!tripOfferId || tripOfferId <= 0) {
        alert('מזהה הצעת מחיר לא תקין');
        return;
    }

    const btn = event?.target?.closest('button');
    if (!btn) {
        console.error('❌ Button element not found');
        return;
    }

    const originalContent = btn.innerHTML;
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>שולח...';

    try {
        console.log(`📤 Sending POST to: /TripOffers/SendEmail/${tripOfferId}`);

        const response = await fetch(`/TripOffers/SendEmail/${tripOfferId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        console.log('📥 Response status:', response.status);

        if (!response.ok) {
            throw new Error(`Server returned ${response.status}: ${response.statusText}`);
        }

        const result = await response.json();
        console.log('✅ Response data:', result);

        if (result.success) {
            showSuccessMessage(result);
        } else {
            alert(`שגיאה: ${result.message || 'לא ידוע'}`);
        }
    } catch (error) {
        console.error('❌ Error:', error);
        alert(`שגיאה בשליחת המייל: ${error.message}`);
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalContent;
    }
}

function showSuccessMessage(data) {
    const overlay = document.createElement('div');
    overlay.style.cssText = `
        position: fixed; top: 0; left: 0; width: 100%; height: 100%;
        background: rgba(0, 0, 0, 0.7); z-index: 9999;
        display: flex; justify-content: center; align-items: center;
    `;

    const card = document.createElement('div');
    card.style.cssText = `
        background: white; border-radius: 20px; padding: 40px;
        max-width: 500px; width: 90%; direction: rtl; text-align: center;
        box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    `;

    card.innerHTML = `
        <div style="font-size: 80px; color: #28a745; margin-bottom: 20px;">✅</div>
        <h2 style="font-size: 28px; font-weight: 700; margin-bottom: 20px;">המייל נשלח בהצלחה!</h2>
        
        <div style="background: #f8f9fa; border-radius: 15px; padding: 20px; margin: 20px 0; text-align: right;">
            <div style="margin-bottom: 15px; padding-bottom: 15px; border-bottom: 1px solid #dee2e6;">
                <div style="font-weight: 600; color: #495057;">תאריך שליחה</div>
                <div style="color: #6c757d; margin-top: 5px;">${data.sentAt || new Date().toLocaleString('he-IL')}</div>
            </div>
            <div style="margin-bottom: 15px; padding-bottom: 15px; border-bottom: 1px solid #dee2e6;">
                <div style="font-weight: 600; color: #495057;">לנמען</div>
                <div style="color: #6c757d; margin-top: 5px; word-break: break-all;">${data.sentTo || '-'}</div>
            </div>
            <div style="margin-bottom: 15px; padding-bottom: 15px; border-bottom: 1px solid #dee2e6;">
                <div style="font-weight: 600; color: #495057;">נושא</div>
                <div style="color: #6c757d; margin-top: 5px; font-size: 14px;">${data.subject || '-'}</div>
            </div>
            <div style="margin-bottom: 15px; padding-bottom: 15px; border-bottom: 1px solid #dee2e6;">
                <div style="font-weight: 600; color: #495057;">מספר הצעה</div>
                <div style="color: #6c757d; margin-top: 5px;">${data.offerNumber || '-'}</div>
            </div>
            <div>
                <div style="font-weight: 600; color: #495057;">שם לקוח</div>
                <div style="color: #6c757d; margin-top: 5px;">${data.customerName || '-'}</div>
            </div>
        </div>
        
        <button onclick="this.closest('.email-success-overlay').remove()" style="
            background: linear-gradient(145deg, #28a745 0%, #20c997 100%);
            color: white; border: none; padding: 15px 35px;
            border-radius: 30px; font-size: 16px; font-weight: 600;
            cursor: pointer; box-shadow: 0 6px 20px rgba(40, 167, 69, 0.3);
        ">סגור</button>
    `;

    overlay.className = 'email-success-overlay';
    overlay.appendChild(card);
    document.body.appendChild(overlay);

    overlay.addEventListener('click', (e) => {
        if (e.target === overlay) overlay.remove();
    });
}

console.log('✅ trip-email.js loaded successfully!');