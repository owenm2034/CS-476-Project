document.querySelectorAll(".watchlist-heart").forEach(btn => {
    btn.addEventListener("click", async function() {
        const itemId = this.dataset.itemid;
        const response = await fetch(`/Watchlist/AddToWatchlist?itemId=${itemId}`, {
            method: "POST"
        });
        if(response.ok){
            // toggle class instantly
            const icon = this.querySelector("i");
            icon.classList.toggle("bi-heart");
            icon.classList.toggle("bi-heart-fill");
            this.classList.toggle("text-danger");
        }
    });
});