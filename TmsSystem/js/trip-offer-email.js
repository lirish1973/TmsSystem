// פונקציה לשליחת הצעת מחיר לטיול
function sendTripOfferEmail(tripOfferId) {
    const btn = event.target;
    const originalText = btn.innerHTML;
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>שולח...';

    fetch(`/TripOffers/SendEmail/${tripOfferId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                btn.innerHTML = '<i class="fas fa-check me-2"></i>נשלח בהצלחה!';
                btn.classList.remove('btn-success');
                btn.classList.add('btn-success');

                alert(`✅ הצעת המחיר נשלחה בהצלחה!\n\nנשלח ל: ${data.sentTo}\nנושא: ${data.subject}\nתאריך: ${data.sentAt}\nמספר הצעה: ${data.offerNumber}`);
            } else {
                btn.innerHTML = '<i class="fas fa-times me-2"></i>נכשל';
                btn.classList.remove('btn-success');
                btn.classList.add('btn-danger');
                alert('❌ שגיאה בשליחה: ' + (data.message || 'לא ידוע'));
            }
        })
        .catch(error => {
            console.error('Error:', error);
            btn.innerHTML = '<i class="fas fa-times me-2"></i>שגיאה';
            btn.classList.remove('btn-success');
            btn.classList.add('btn-danger');
            alert('❌ שגיאה בשליחה: ' + error);
        })
        .finally(() => {
            setTimeout(() => {
                btn.innerHTML = originalText;
                btn.disabled = false;
                btn.classList.remove('btn-danger');
                btn.classList.add('btn-success');
            }, 3000);
        });
}