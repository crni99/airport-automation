(function waitForAuthWrapper() {
    const authWrapper = document.querySelector('div.auth-wrapper');
    if (!authWrapper) {
        setTimeout(waitForAuthWrapper, 100);
        return;
    }

    const authorizeButton = authWrapper.querySelector('button');
    if (!authorizeButton) {
        setTimeout(waitForAuthWrapper, 100);
        return;
    }

    const button = document.createElement('button');
    button.classList.add('btn');
    button.style.marginRight = '20px';

    const darkThemeHref = '/swagger-ui/swagger-dark.css';
    let isDark = false;

    function enableDarkTheme() {
        if (!document.querySelector(`link[href="${darkThemeHref}"]`)) {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = darkThemeHref;
            document.head.appendChild(link);
        }
        isDark = true;
        button.innerText = 'Light Mode ☀️';
    }

    function disableDarkTheme() {
        const existingLink = document.querySelector(`link[href="${darkThemeHref}"]`);
        if (existingLink) {
            existingLink.remove();
        }
        isDark = false;
        button.innerText = 'Dark Mode 🌙';
    }

    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        enableDarkTheme();
    } else {
        disableDarkTheme();
    }

    if (!button.innerText) {
        button.innerText = 'Dark Mode 🌙';
    }

    authWrapper.insertBefore(button, authorizeButton);

    button.onclick = () => {
        if (isDark) {
            disableDarkTheme();
        } else {
            enableDarkTheme();
        }
    };

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
        if (e.matches) {
            enableDarkTheme();
        } else {
            disableDarkTheme();
        }
    });
})();

(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            var existingLinks = document.querySelectorAll("link[rel*='icon']");
            existingLinks.forEach(function (link) {
                link.remove();
            });

            var link32 = document.createElement('link');
            link32.rel = 'icon';
            link32.type = 'image/x-icon';
            link32.href = '/swagger-ui/favicon.ico';
            link32.sizes = '32x32';
            document.head.appendChild(link32);

            // Inject your new 16x16 favicon
            var link16 = document.createElement('link');
            link16.rel = 'icon';
            link16.type = 'image/x-icon';
            link16.href = '/swagger-ui/favicon.ico';
            link16.sizes = '16x16';
            document.head.appendChild(link16);
        }, 100);
    });
})();