document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.btn-delete-trigger').forEach(button => {
        button.addEventListener('click', function () {
            const listingId = this.getAttribute('data-listing-id');
            const action = this.getAttribute('data-form-action');
            const returnTo = this.getAttribute('data-return-to') ?? '';

            let formAction = `${action}?listingId=${listingId}`;
            if (returnTo) formAction += `&returnTo=${returnTo}`;

            document.getElementById('deleteForm').action = formAction;
            new bootstrap.Modal(document.getElementById('deleteModal')).show();
        });
    });
});