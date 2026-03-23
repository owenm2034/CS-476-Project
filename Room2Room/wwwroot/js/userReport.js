function openUserReportModal(accountId) {
    document.getElementById('reportAccountId').value = accountId;
    document.getElementById('userReportReason').value = '';
    document.getElementById('userReportFeedback').className = 'd-none';
    document.getElementById('userReportFeedback').textContent = '';
    new bootstrap.Modal(document.getElementById('userReportModal')).show();
}

function submitUserReport() {
    const accountId = document.getElementById('reportAccountId').value;
    const reason = document.getElementById('userReportReason').value.trim();
    const feedback = document.getElementById('userReportFeedback');

    if (!reason) {
        feedback.textContent = 'Please enter a reason before submitting.';
        feedback.className = 'alert alert-warning mt-2';
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    fetch('/Account/Report', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `accountId=${accountId}&reason=${encodeURIComponent(reason)}&__RequestVerificationToken=${encodeURIComponent(token)}`
    })
    .then(r => { if (!r.ok) throw new Error(); return r.json(); })
    .then(() => {
        feedback.textContent = '✅ Report submitted. Thank you.';
        feedback.className = 'alert alert-success mt-2';
        document.getElementById('userReportReason').disabled = true;
        document.querySelector('#userReportModal .btn-danger').disabled = true;
        setTimeout(() => {
            bootstrap.Modal.getInstance(document.getElementById('userReportModal')).hide();
        }, 1000);
    })
    .catch(() => {
        feedback.textContent = ' Something went wrong. Please try again.';
        feedback.className = 'alert alert-danger mt-2';
    });
}