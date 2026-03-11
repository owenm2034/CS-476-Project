// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// ===== Announcement banner =====
async function openChat() {
    const res = await fetch('/Chat/', {
        method: 'GET',
    });

    if (res.ok) {
        // Close the modal and optionally show a success message
        const data = await res.text();
        document.getElementById("chat-modal").innerHTML = data;
        registerChatEvents();
        setTimeout(pollChats, 5000);

        // this ensures the chat container scrolls to the bottom when opening a chat
        setTimeout(() => {
            var nodes = Array.from(document.getElementsByClassName("chat-message-container"));
            nodes.forEach(x => {
                x.scrollTop = x.scrollHeight;
            });
        }, 500);
    }
}

let lastUpdated = new Date().toISOString();

function registerChatEvents() {
    var elements = document.getElementsByClassName("chatSidebarContainer")

    for (let item of elements) {
        item.addEventListener("click", function(x) {
            var chatId = item.firstChild.nextElementSibling.attributes["data-chatid"].value
            item.parentElement.querySelectorAll('.active').forEach(el => el.classList.remove('active'));
            Array.from(document.getElementsByClassName('chat-message-container')).forEach(el => {el.style.visibility = 'hidden'; el.style.display = 'none' });
            item.firstChild.nextElementSibling.classList.add("active");
            document.getElementById("chat-messages-" + chatId).style.visibility = "visible";
            document.getElementById("chat-messages-" + chatId).style.display = "flex";
        })
    }
}

async function sendChat(chatId) {
    var box = document.getElementById("chat-box-for-chat-" + chatId)
    var message = box.value

    const formData = new URLSearchParams();
    formData.append('message.ChatId', chatId);
    formData.append('message.Message', message);

    const res = await fetch('/Chat/SendMessage', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData.toString()
    });

    if (!res.ok) return;

    var resp = await res.text();

    var toReplace = document.getElementById("chat-messages-" + chatId);
    toReplace.innerHTML = resp;
    toReplace.scrollTop = toReplace.scrollHeight;
    box.value = ""
    lastUpdated = new Date().toISOString();
}

async function pollChats() {
    var chatContainers = document.getElementsByClassName("chat-message-container")

    const res = await fetch('/Chat/GetNewMessages?since=' + lastUpdated, {
        method: 'GET',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    });

    var html = await res.text();
    const parser = new DOMParser();
    const doc = parser.parseFromString(html, "text/html");
    var newChats = Array.from(doc.getElementsByClassName("chat-message-container"));

    for (let item of newChats) {
        var target = document.getElementById(item.id);
        if (!target) {
            continue;
        }

        while (item.firstChild) {
            let node = item.firstChild;
            if (node.classList && node.classList.contains("chat-box-for-chat")) {
                item.removeChild(node);
                continue;
            }

            target.appendChild(node);
        }

        const el = target.querySelector(".chat-box-for-chat");
        target.appendChild(el);
        target.scrollTop = target.scrollHeight;
    }

    lastUpdated = new Date().toISOString();
    setTimeout(pollChats, 5000);
}

async function sendFirstMessage() {
    // get element
    var modalElement = document.getElementById('newChatModal');
    var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    var listingId = modalElement.attributes['data-itemid'];
    modal.hide();
    
    var message = document.getElementById('newChatMessageInput').value;
    
    // call c# endpoint
    const formData = new URLSearchParams();
    formData.append('message.ListingId', listingId);
    formData.append('message.Message', message);

    const res = await fetch('/Chat/SendFirstMessage', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData.toString()
    });
    lastUpdated = new Date().toISOString();

    // open big chat window
    const data = await res.text();
    document.getElementById("chat-modal").innerHTML = data;
    registerChatEvents();
    setTimeout(pollChats, 5000);
    var modalElement = document.getElementById('chatModal');
    var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.show();

    // this ensures the chat container scrolls to the bottom when opening a chat
    setTimeout(() => {
        var nodes = Array.from(document.getElementsByClassName("chat-message-container"));
        nodes.forEach(x => {
            x.scrollTop = x.scrollHeight;
        });
    }, 500);
    // active on the chat that was just sent -> maybe the c# endpoint can return the chat modal content?????

}

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