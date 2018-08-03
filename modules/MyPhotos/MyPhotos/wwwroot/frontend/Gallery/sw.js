const PRECACHE = 'precache-v1.12';
const RUNTIME = 'runtime-v1.12';


const CACHE_FIRST_URLS = [
'/api/'
];

// A list of local resources we always want to be cached.
const PRECACHE_URLS = [
  './index.z',
  './gallery.z'
];

self.addEventListener('install', event => {
  console.log("Fire install event");
  event.waitUntil(
    caches.open(PRECACHE)
      .then(cache => cache.addAll(PRECACHE_URLS))
      .then(self.skipWaiting())
  );
});

self.addEventListener('activate', event => {
console.log("Call Activate");
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
  // Skip cross-origin requests, like those for Google Analytics.
  if (event.request.url.startsWith(self.location.origin)) {
    event.respondWith(

      caches.match(event.request).then(cachedResponse => {
          console.log("Get cache",event.request,event.request.url ,cachedResponse);
        if (cachedResponse) {
          return cachedResponse;
        }

        return caches.open(RUNTIME).then(cache => {
        console.log("fetch, response from network",event.request.url);
          return fetch(event.request).then(response => {
            console.log("fetch, response from network",event.request,event.request.url ,response);
              if(response.status == 200){
                    return cache.put(event.request, response.clone()).then(() => {
                      return response;
                    });
              }
              else{
                  return response;
              }
          });
        });
      })
    );
  }
});
