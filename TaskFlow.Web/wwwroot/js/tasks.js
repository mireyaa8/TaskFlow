async function changeStatus(taskId, status) {
    const response = await fetch(`/api/tasks/${taskId}/status`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ status })
    });

    if (response.ok) {
        window.location.reload();
    } else {
        alert('Could not update task status.');
    }
}

const searchBox = document.getElementById('taskSearch');
if (searchBox) {
    searchBox.addEventListener('input', async function () {
        const response = await fetch(`/api/tasks/search?keyword=${encodeURIComponent(searchBox.value)}`);
        const tasks = await response.json();
        const container = document.getElementById('taskResults');
        container.innerHTML = tasks.map(t => `
            <div class="col-md-4 mb-3 task-card">
                <div class="card h-100"><div class="card-body">
                    <h5>${escapeHtml(t.title)}</h5>
                    <p>${escapeHtml(t.description)}</p>
                    <span class="badge bg-secondary">${escapeHtml(t.status)}</span>
                    <span class="badge bg-info">${escapeHtml(t.priority)}</span>
                    <div class="mt-3"><a class="btn btn-sm btn-outline-primary" href="/Tasks/Details/${t.id}">Details</a></div>
                </div></div>
            </div>`).join('');
    });
}

function escapeHtml(value) {
    return String(value).replace(/[&<>'"]/g, function (char) {
        return ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', "'": '&#39;', '"': '&quot;' })[char];
    });
}
