document.addEventListener("DOMContentLoaded", function () {
    const signOutButton = document.getElementById("signOut");
    if (signOutButton) {
        signOutButton.addEventListener("click", function (e) {
            e.preventDefault();

            fetch('/SignOut', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    if (!response.ok) {
                        if (response.status === 401) {
                            throw new Error('Unauthorized: Please log in.');
                        } else {
                            throw new Error('Network response was not ok');
                        }
                    }
                    return response.json();
                })
                .then(data => {
                    if (data.success) {
                        window.location.href = '/?logout=true';
                    } else {
                        alert('Sign out failed. Please try again later or contact support for assistance.');
                        throw new Error('Sign out failed');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        });
    }
});
