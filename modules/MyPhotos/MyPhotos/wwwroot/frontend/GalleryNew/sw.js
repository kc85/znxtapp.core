const PRECACHE = 'precache-v1.385';
const RUNTIME = 'runtime-v1.385';


const BACKGROUND_CACHE_URLS = [
    '/api/myphotos/gallery',
    '/indexnew.z',
    '/api/myphotos/user/bookmark',
    '/api/myphotos/users'
]

const IGNORE_CACHE_URLS = [
    '/api/user/userinfo',
    '/api/user/me',
    '/api/myphotos/image/rotate',
    '/api/myphotos/fetch',
    '/clearcache.z',
    '/api/user/google/auth',
    '/api/myphotos/image/like',
    '/api/myphotos/image/bookmark',
    '/api/myphotos/gallery/update',
    'https://apis.google.com/js/platform.js',
    '/api/myphotos/gallery/addimage',
    '/api/myphotos/gallery/deleteimage'
];

// A list of local resources we always want to be cached.
const PRECACHE_URLS = [
    './index.z',
    './gallery.z'
];



self.addEventListener('install', event => {
    //console.log("Fire install event");
event.waitUntil(
    caches.open(PRECACHE)
    .then(cache => cache.addAll(PRECACHE_URLS))
    .then(self.skipWaiting())
);
});

self.addEventListener('activate', event => {
    //console.log("Call Activate");
const currentCaches = [PRECACHE, RUNTIME];
event.waitUntil(
    caches.keys().then(cacheNames => {
        return cacheNames.filter(cacheName => !currentCaches.includes(cacheName));
}).then(cachesToDelete => {
    return Promise.all(cachesToDelete.map(cacheToDelete => {
        return caches.delete(cacheToDelete);
}));
}).then(() => self.clients.claim())
    );
});


self.addEventListener('fetch', event => {
    // console.log("fetch",event.request);

    // Prevent the default, and handle the request ourselves.
    event.respondWith(async function() {
        // Try to get the response from a cache.
        var cloneRequest = event.request.clone();
        const cachedResponse = await caches.match(event.request);
        // Return it if we found one.
        if (cachedResponse) {
            //console.log("response from cache", event.request.url);
            cacheInBackground(event.request.clone());
            return cachedResponse;

        }
        // If we didn't find a match in the cache, use the network.
        return fetch(event.request).then(
            function(reponse) {
                addToCache(cloneRequest, reponse.clone())
                return reponse;
            });
    }());
});

function addToCache(cloneRequest, cloneRespose) {
    var isRequired = true;
    for (var i = 0; i < IGNORE_CACHE_URLS.length; i++) {
        var url = IGNORE_CACHE_URLS[i];
        if (cloneRequest.url.toLowerCase().indexOf(url) != -1) {
            isRequired = false;
        }
    }

    if (isRequired) {
        addToRunTimeCache(cloneRequest, cloneRespose);
    }
}

function cacheInBackground(cloneRequest) {

    var isRequired = false;
    for (var i = 0; i < BACKGROUND_CACHE_URLS.length; i++) {
        var url = BACKGROUND_CACHE_URLS[i];
        if (cloneRequest.url.toLowerCase().indexOf(url) != -1) {
            isRequired = true;
        }
    }

    if (isRequired) {
        fetch(cloneRequest).then(
            function(reponse) {
                addToCache(cloneRequest, reponse);
            });
    }

}

function addToRunTimeCache(request, response) {
    if (!response || response.status !== 200 || response.type !== 'basic') {
        return response;
    } else {

        caches.open(RUNTIME)
            .then(function(cache) {
                //console.log("Cached in background ", request.url, response);
                cache.put(request, response);
            });

    }
}
