document.addEventListener("DOMContentLoaded", function() {
    const listingTab = document.getElementById("listing-tab");

    if (listingTab) {
        listingTab.addEventListener("click", function () {
            loadAdminListings();
        });
    }
});

function loadAdminListings() {
    fetch('/Admin/Listings')
        .then(response => {
            if (!response.ok) throw new Error("Network response was not ok");
            return response.text();
        })
        .then(html => {
            const container = document.getElementById('adminListingsContainer');
            if (container) container.innerHTML = html;
        })
        .catch(error => {
            console.error('Error loading admin listings:', error);
        });
}