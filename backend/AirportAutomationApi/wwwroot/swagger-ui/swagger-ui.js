(function waitForFaviconLinks() {
    const swaggerContainer = document.querySelector('#swagger-ui');
    if (!swaggerContainer) {
        setTimeout(waitForFaviconLinks, 100);
        return;
    }

    const existingLinks = document.querySelectorAll("link[rel*='icon']");
    existingLinks.forEach(function (link) {
        link.remove();
    });

    const link32 = document.createElement('link');
    link32.rel = 'icon';
    link32.type = 'image/x-icon';
    link32.href = '/swagger-ui/favicon.ico';
    link32.sizes = '32x32';
    document.head.appendChild(link32);

    const link16 = document.createElement('link');
    link16.rel = 'icon';
    link16.type = 'image/x-icon';
    link16.href = '/swagger-ui/favicon.ico';
    link16.sizes = '16x16';
    document.head.appendChild(link16);
})();

(function observeContactDiv() {
    const targetSelector = '.info__contact';
    const swaggerUiContainer = document.getElementById('swagger-ui');

    if (!swaggerUiContainer) {
        setTimeout(observeContactDiv, 100);
        return;
    }

    function modifyContactInfo(contactDiv, observer) {
        const websiteWrapperDiv = contactDiv.querySelector('div');

        if (!websiteWrapperDiv) {
            return;
        }

        websiteWrapperDiv.classList.add('contact-link');
        const websiteLink = websiteWrapperDiv.querySelector('a');

        if (websiteLink) {
            websiteLink.href = "https://www.linkedin.com/in/ognj3n";
            websiteLink.innerHTML = `Ognjen Andjelic ─ LinkedIn`;

            const githubWrapperDiv = document.createElement('div');
            githubWrapperDiv.classList.add('contact-link');
            const githubLink = document.createElement('a');

            githubLink.href = "https://github.com/crni99";
            githubLink.target = "_blank";
            githubLink.rel = "noopener noreferrer";
            githubLink.classList.add("link");
            githubLink.innerHTML = `Ognjen Andjelic ─ GitHub`;

            githubWrapperDiv.appendChild(githubLink);

            if (websiteWrapperDiv.nextElementSibling) {
                contactDiv.insertBefore(githubWrapperDiv, websiteWrapperDiv.nextElementSibling);
            } else {
                contactDiv.appendChild(githubWrapperDiv);
            }
        }
        if (observer) observer.disconnect();
    }

    const initialContactDiv = swaggerUiContainer.querySelector(targetSelector);
    if (initialContactDiv) {
        modifyContactInfo(initialContactDiv, null);
        return;
    }

    const observer = new MutationObserver(function (mutations, obs) {
        const contactDiv = swaggerUiContainer.querySelector(targetSelector);
        if (contactDiv) {
            modifyContactInfo(contactDiv, obs);
        }
    });

    observer.observe(swaggerUiContainer, {
        childList: true,
        subtree: true
    });
})();

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