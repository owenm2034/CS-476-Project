// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// ===== Announcement banner =====
(function () {
    async function updateBanner() {
        try {
            const r = await fetch('/Announcement/Active', { cache: "no-store" });
            const a = await r.json();

            const container = document.getElementById("announcementContainer");
            const banner = document.getElementById("announcementBanner");
            if (!container || !banner) return;

            const message = (a && (a.message || a.Message)) ? (a.message || a.Message) : "";
            if (message.trim().length === 0) {
                container.style.display = "none";
                banner.textContent = "";
                return;
            }

            container.style.display = "block";

            banner.className = "alert alert-warning mt-2 mb-2";
            banner.textContent = message;
        } catch (e) {
            // If request fails, hide banner
            const container = document.getElementById("announcementContainer");
            const banner = document.getElementById("announcementBanner");
            if (container && banner) {
                container.style.display = "none";
                banner.textContent = "";
            }
        }
    }

    function run() {
        updateBanner();
        // Auto-refresh for "timed" behavior while user stays on the page
        setInterval(updateBanner, 30000); // every 30 seconds
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", run);
    } else {
        run();
    }
})();
// ===== End Announcement banner =====