// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// ===== Announcement banner =====
(function () {
    function run() {
        fetch('/Announcement/Active')
            .then(r => r.json())
            .then(a => {
                if (!a) return;

                const container = document.getElementById("announcementContainer");
                const banner = document.getElementById("announcementBanner");
                if (!container || !banner) return;

                const message = a.message || a.Message || "";
                if (message.trim().length === 0) return;

                container.style.display = "block";

                const color = (a.color || a.Color || "warning").toLowerCase();
                banner.className = "alert alert-" + color + " mt-2 mb-2";
                banner.textContent = message;
            })
            .catch(() => {});
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", run);
    } else {
        run();
    }
})();
