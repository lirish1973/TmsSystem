// פונקציה לשליחת הצעת מחיר לטיול
function sendTripProposalEmail(tripId) {
    const email = prompt('הזן כתובת אימייל של הלקוח:');

    if (!email) {
        return;
    }

    if (!validateEmail(email)) {
        alert('כתובת אימייל לא תקינה');
        return;
    }

    const customerName = prompt('הזן שם לקוח (אופציונלי):');

    const btn = event.target;
    const originalText = btn.innerHTML;
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> שולח...';

    const formData = new FormData();
    formData.append('email', email);
    if (customerName) {
        formData.append('customerName', customerName);
    }

    fetch(`/trips/${tripId}/send-email`, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                btn.innerHTML = '<i class="fas fa-check"></i> נשלח בהצלחה!';
                btn.classList.remove('btn-primary');
                btn.classList.add('btn-success');

                alert(`הצעת המחיר נשלחה בהצלחה ל: ${data.sentTo}\n\nנושא: ${data.subject}\nתאריך: ${data.sentAt}`);
            } else {
                btn.innerHTML = '<i class="fas fa-times"></i> נכשל';
                btn.classList.remove('btn-primary');
                btn.classList.add('btn-danger');
                alert('שגיאה בשליחה: ' + (data.message || 'לא ידוע'));
            }
        })
        .catch(error => {
            console.error('Error:', error);
            btn.innerHTML = '<i class="fas fa-times"></i> שגיאה';
            btn.classList.remove('btn-primary');
            btn.classList.add('btn-danger');
            alert('שגיאה בשליחה: ' + error);
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

function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}