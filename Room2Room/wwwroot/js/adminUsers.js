document.addEventListener("DOMContentLoaded", function() {
    const userTab = document.getElementById("user-tab");

    if (userTab) {
        userTab.addEventListener("click", function () {
            loadAdminUsers();
        });
    }
});

function loadAdminUsers() {
    fetch("/Admin/Users")
        .then(response => {
            if (!response.ok) throw new Error("Network response was not ok");
            return response.text();
        })
        .then(html => {
            const container = document.getElementById("adminUsersContainer");
            if (container) container.innerHTML = html;
        })
        .catch(error => {
            console.error("Error loading admin users:", error);
        });
}

function searchAdminUsers() {
    const searchTerm = document.getElementById("adminUserSearchTerm")?.value || "";
    let url = "/Admin/Users";

    if (searchTerm) {
        url += "?sTerm=" + encodeURIComponent(searchTerm);
    }

    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error("Network response was not ok");
            return response.text();
        })
        .then(html => {
            const container = document.getElementById("adminUsersContainer");
            if (container) container.innerHTML = html;
        })
        .catch(error => {
            console.error("Error searching admin users:", error);
        });
}
