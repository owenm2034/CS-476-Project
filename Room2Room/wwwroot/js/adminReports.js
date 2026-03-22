async function loadReports() {
    const currentFilter = document.querySelector('#adminReportsContainer select')?.value ?? 'all';
    
    const res = await fetch('/Admin/ListReports', { cache: 'no-store' });
    document.getElementById('adminReportsContainer').innerHTML = await res.text();

    const select = document.querySelector('#adminReportsContainer select');
    if (select) {
        filterReports(select.value);
    }
}

async function resolveReport(id) {
    const noteEl = document.getElementById('note-' + id);
    const errorEl = document.getElementById('note-error-' + id);
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

    const res = await fetch('/Admin/ResolveReport', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData.toString()
    });

    if (!res.ok) { alert('Failed to resolve report.'); return; }
    await loadReports();
}

function filterReports(type) {

    document.querySelectorAll('tr[id^="report-row-"]').forEach(row => {
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
    const tab = document.getElementById('reports-tab');
    if (tab) {
        tab.addEventListener('shown.bs.tab', () => loadReports());
    }
});