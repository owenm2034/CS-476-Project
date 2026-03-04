document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll('.watchlist-heart').forEach(button => {
        button.addEventListener('click', async function (e) {
            e.preventDefault();

            const itemId = this.dataset.itemId;

            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            const response = await fetch('/Watchlist/AddToWatchlist', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: `itemId=${itemId}`
            });

            const data = await response.json();

            if (data.success) {
                const icon = this.querySelector('i');

                if (data.inWatchlist) {
                    icon.classList.remove('bi-heart');
                    icon.classList.add('bi-heart-fill');
                } else {
                    icon.classList.remove('bi-heart-fill');
                    icon.classList.add('bi-heart');
                }
            }
        });
    });

});