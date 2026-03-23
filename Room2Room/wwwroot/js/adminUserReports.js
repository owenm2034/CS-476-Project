async function loadUserReports() {
    const currentFilter = document.querySelector('#adminUserReportsContainer select')?.value ?? 'all';

    const res = await fetch('/Admin/ListUserReports', { cache: 'no-store' });
    document.getElementById('adminUserReportsContainer').innerHTML = await res.text();

    const select = document.querySelector('#adminUserReportsContainer select');
    if (select) {
        select.value = currentFilter;
        filterUserReports(currentFilter);
    }
}

async function resolveUserReport(id) {
    const noteEl = document.getElementById('user-note-' + id);
    const errorEl = document.getElementById('user-note-error-' + id);
    const note = noteEl ? noteEl.value.trim() : '';

    if (!note) {
        noteEl.classList.add('is-invalid');
        errorEl.classList.remove('d-none');
        return;
    }

    noteEl.classList.remove('is-invalid');
    errorEl.classList.add('d-none');

    const formData = new URLSearchParams();
    formData.append('id', id);
    formData.append('resolution', note);

    const res = await fetch('/Admin/ResolveUserReport', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData.toString()
    });

    if (!res.ok) { alert('Failed to resolve report.'); return; }
    await loadUserReports();
}

function filterUserReports(type) {
    document.querySelectorAll('tr[id^="user-report-row-"]').forEach(row => {
        const resolved = row.dataset.resolved === 'true';
        if (type === 'all') {
            row.style.display = '';
        } else if (type === 'resolved') {
            row.style.display = resolved ? '' : 'none';
        } else if (type === 'unresolved') {
            row.style.display = !resolved ? '' : 'none';
        }
    });
}

document.addEventListener('DOMContentLoaded', () => {
    const tab = document.getElementById('user-reports-tab');
    if (tab) {
        tab.addEventListener('shown.bs.tab', () => loadUserReports());
    }
});