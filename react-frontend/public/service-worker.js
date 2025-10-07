const CACHE_NAME = 'airport-automation-v1';
const urlsToCache = [
    '/',
    '/index.html',
    '/manifest.json',
    '/favicon.ico',
    '/images/icons/16x16.png',
    '/images/icons/32x32.png',
    '/images/icons/72x72.png',
    '/images/icons/96x96.png',
    '/images/icons/120x120.png',
    '/images/icons/128x128.png',
    '/images/icons/144x144.png',
    '/images/icons/152x152.png',
    '/images/icons/180x180.png',
    '/images/icons/192x192.png',
    '/images/icons/384x384.png',
    '/images/icons/512x512.png'
    // Add your bundled JavaScript and CSS files here (e.g., /static/js/main.js, /static/css/main.css)
];

self.addEventListener('install', (event) => {
    console.log('[Service Worker] Install');
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then((cache) => {
                console.log('[Service Worker] Pre-caching all app shell content');
                return cache.addAll(urlsToCache);
            })
    );
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    console.log('[Service Worker] Activate');
    const cacheWhitelist = [CACHE_NAME];
    event.waitUntil(
        caches.keys().then((cacheNames) => {
            return Promise.all(
                cacheNames.map((cacheName) => {
                    if (cacheWhitelist.indexOf(cacheName) === -1) {
                        console.log('[Service Worker] Deleting old cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                    return Promise.resolve();
                })
            );
        })
    );
    return self.clients.claim();
});

self.addEventListener('fetch', (event) => {
    event.respondWith(
        caches.match(event.request)
            .then((response) => {
                if (response) {
                    return response;
                }
                return fetch(event.request);
            })
    );
});