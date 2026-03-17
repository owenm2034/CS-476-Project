async function viewChat(userId, name) {
    await openChat(userId);
    
    var modalElement = document.getElementById('chatModal');
    var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.show();

    document.getElementById("chat-modal-header").textContent = name + "'s Chats";
}