document.addEventListener("DOMContentLoaded", function() {
    const statisticsTab = document.getElementById("statistics-tab");

    if (statisticsTab) {
        statisticsTab.addEventListener("click", function () {
            loadAdminStatistics();
        });
    }
});

function loadAdminStatistics() {
    fetch('/Admin/Statistics')
        .then(response => {
            if (!response.ok) throw new Error("Network response was not ok");
            return response.text();
        })
        .then(html => {
            const container = document.getElementById('adminStatisticsContainer');
            if (container) container.innerHTML = html;
        })
        .catch(error => {
            console.error('Error loading admin statistics:', error);
        });
}