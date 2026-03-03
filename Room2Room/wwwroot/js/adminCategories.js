document.addEventListener("DOMContentLoaded", function() {
    const categoryTab = document.getElementById("listing-categories-tab");

    if (categoryTab) {
        categoryTab.addEventListener("click", function () {
            loadCategories();
        });
    }
});

function loadCategories() {
    fetch('/Admin/GetCategories')
        .then(response => {
            if (!response.ok) throw new Error("Network response was not ok");
            return response.text();
        })
        .then(html => {
            const container = document.getElementById('adminCategoryContainer');
            if (container) container.innerHTML = html;
        })
        .catch(error => {
            console.error('Error loading admin listings:', error);
        });
}