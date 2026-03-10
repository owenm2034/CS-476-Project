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

    (async () => {
        await openChat();
    })();

});

async function openItemChat(itemId) {
    var el = document.querySelector('[data-listingid="' + itemId + '"]');
    if (el != null) {
        console.log("chat exists");
        await openChat();
        var chatId = el.attributes["data-chatid"].value
        var listGroup = document.querySelector('.list-group');
        if (listGroup && listGroup.parentElement) {
            listGroup.parentElement.querySelectorAll('.active').forEach(el => el.classList.remove('active'));
        }
        Array.from(document.getElementsByClassName('chat-message-container')).forEach(el => {el.style.visibility = 'hidden'; el.style.display = 'none' });
        el.classList.add("active");
        document.getElementById("chat-messages-" + chatId).style.visibility = "visible";
        document.getElementById("chat-messages-" + chatId).style.display = "flex";
    }
}