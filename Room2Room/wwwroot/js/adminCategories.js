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

async function upsertCategory(catId) {
    var el;
    if (!catId) {
        el = document.getElementById("new-category-name")
    } else {
        el = document.getElementById("category-name-" + catId);
    }
    
    var name = el.value;

    const formData = new URLSearchParams();
    formData.append('cat.Id', catId);
    formData.append('cat.CategoryName', name);

    const res = await fetch('/Admin/UpsertCategory', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData.toString()
    });
    
    loadCategories();
}

async function deleteCategory(catId) {
    const formData = new URLSearchParams();
    formData.append('catId', catId);

    const res = await fetch('/Admin/DeleteCategory', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData.toString()
    });
    
    loadCategories();
}