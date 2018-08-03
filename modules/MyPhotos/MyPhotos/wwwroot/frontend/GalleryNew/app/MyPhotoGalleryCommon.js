

function GetParameterValues(param) {
    var url = window.location.href.replace(window.location.hash,"").slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}

function removeCacheKey(url,callback) {
    function removeCache(cacheName, url, callbackparent) {
        return window.caches.open(cacheName).then(function (cache) {
            cache.delete(url);
            callbackparent();
        });
    }
    window.caches.keys().then(function (cacheNames) {
        cacheNames.forEach(function (cacheName) {
            window.caches.open(cacheName).then(function (cache) {
                return cache.keys();
            }).then(function (requests) {
                requests.forEach(function (r) {
                    if (r.url.indexOf(url) != -1) {
                        console.log(r.url);
                        removeCache(cacheName, r.url, callback);
                    }
                });
                callback();
            });

        });
    });
}
var lastFileHash = "";
function replaceImage(image) {
    var changeset = $(image).attr("changeset");
    var fileHash = $(image).attr("file_hash");
    if (lastFileHash != fileHash) {
        lastFileHash = fileHash;
        var mainImage = image;
        var imageL = new Image();
        imageL.onload = function () {
            console.log("Large image Loaded ... ", this.src);
            mainImage.src = this.src;
        }
        imageL.src = "../api/myphotos/image?file_hash=" + fileHash + "&t=l&changeset_no=" + changeset;
    }
}

function showProfile() {
    if (window.__userData == undefined) {
        showLogin();
    }
    else {
        window.location = "./userprofile.z"
    }
    
};
function showLogin() {
    $("#myphotoLogin").modal("show");
}

var isMobile = {
    Android: function () {
        return navigator.userAgent.match(/Android/i);
    },
    BlackBerry: function () {
        return navigator.userAgent.match(/BlackBerry/i);
    },
    iOS: function () {
        return navigator.userAgent.match(/iPhone|iPad|iPod/i);
    },
    Opera: function () {
        return navigator.userAgent.match(/Opera Mini/i);
    },
    Windows: function () {
        return navigator.userAgent.match(/IEMobile/i);
    },
    any: function () {
        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
    }
};

$(document).ready(function () {


    $(document).on("click", '.whatsapp', function () {
        if (isMobile.any()) {
            var text = $(this).attr("data-text");
            var url = $(this).attr("data-link");
            var message = encodeURIComponent(text) + " - " + encodeURIComponent(url);
            var whatsapp_url = "whatsapp://send?text=" + message;
            window.location.href = whatsapp_url;
        } else {
            alert("Please share this article in mobile device");
        }
    });


    // Scroll to top When imageViewDetails Modal show 

    $('#imageViewDetails').on('shown.bs.modal', function () {
        $('#btnShareImage').trigger('focus');
    })

    function getUserInfo(){
        $.get("../api/myphotos/userinfo", function (response) {
            window.__userData = response.data;

        })
    }
    getUserInfo();
});