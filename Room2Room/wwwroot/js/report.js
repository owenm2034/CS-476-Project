function openReportModal(itemId) {
    document.getElementById('reportItemId').value = itemId;
    document.getElementById('reportReason').value = '';
    document.getElementById('reportFeedback').classList.add('d-none');
    document.getElementById('reportFeedback').textContent = '';
    new bootstrap.Modal(document.getElementById('reportModal')).show();
}

function submitReport() {
    const itemId = document.getElementById('reportItemId').value;
    const reason = document.getElementById('reportReason').value.trim();
    const feedback = document.getElementById('reportFeedback');

    if (!reason) {
        feedback.textContent = 'Please enter a reason before submitting.';
        feedback.className = 'alert alert-warning mt-2';
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    fetch('/Listing/Report', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `itemId=${itemId}&reason=${encodeURIComponent(reason)}&__RequestVerificationToken=${encodeURIComponent(token)}`
    })
    .then(r => {
        if (!r.ok) throw new Error();
        return r.json();
    })
    .then(() => {
        feedback.textContent = 'Report submitted. Thank you.';
        feedback.className = 'alert alert-success mt-2';
        document.getElementById('reportReason').value = '';
        document.getElementById('reportReason').disabled = true;
        document.querySelector('#reportModal .btn-danger').disabled = true;

        setTimeout(() => {
        bootstrap.Modal.getInstance(document.getElementById('reportModal')).hide();
    }, 1500);
    })
    .catch(() => {
        feedback.textContent = 'Something went wrong. Please try again.';
        feedback.className = 'alert alert-danger mt-2';
    });
}