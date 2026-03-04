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

function searchAdminListings() {
    const searchTerm = document.getElementById("adminsterm").value;
    const categoryId = document.getElementById("adminCategoryId").value;
    const universityId = document.getElementById("adminUniversityId").value;
    var url = '/Admin/Listings';

    if (searchTerm || categoryId || universityId) {
        url += "?";
    }

    // todo: make model driven
    const params = new URLSearchParams();
    if (searchTerm) {
        url += "sTerm=" +searchTerm + "&";
    } 
    if (categoryId) {
        url += "categoryId=" +categoryId + "&";
    }
    if (universityId) {
        url += "universityId=" +universityId + "&";
    }

        fetch(encodeURI(url))
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