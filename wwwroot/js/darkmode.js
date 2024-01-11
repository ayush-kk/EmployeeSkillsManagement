// darkMode.js
document.addEventListener('DOMContentLoaded', function () {
    const darkModeButton = document.getElementById('darkModeButton');
    const body = document.body;

    // Check if dark mode is already enabled (e.g., if the user has a preference)
    const isDarkModeEnabled = localStorage.getItem('darkMode') === 'enabled';

    if (isDarkModeEnabled) {
        body.classList.add('dark-mode');
    }

    darkModeButton.addEventListener('click', function () {
        // Toggle the dark mode class on the body
        body.classList.toggle('dark-mode');

        // Update the dark mode state in localStorage
        const isDarkModeActive = body.classList.contains('dark-mode');
        localStorage.setItem('darkMode', isDarkModeActive ? 'enabled' : 'disabled');
    });
});
