
var userlogincookiekey ="userlogin"
let deferredPrompt;

window.addEventListener('beforeinstallprompt', (e) => {

    console.log("Fire beforeinstallprompt");
    // Prevent Chrome 67 and earlier from automatically showing the prompt
    e.preventDefault();
    // Stash the event so it can be triggered later.
    deferredPrompt = e;
    $("#addToHomeScreenModal").modal("show");
});





function GetParameterValues(param) {
    var url = window.location.search.slice(window.location.search.indexOf('?') + 1).split('&');
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
            if (callbackparent != undefined) {
                callbackparent();
            }
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
                       return removeCache(cacheName, r.url, callback);
                    }
                });
                
            });

        });
    });

    if (callback != undefined) {
        callback();
    }
}
var lastFileHash = "";
function replaceImage(image) {
    var changeset = $(image).attr("changeset");
    var fileHash = $(image).attr("file_hash");
    if (lastFileHash != fileHash) {
        lastFileHash = fileHash;
        var mainImage = image;
        var imageL = new Image();
        $(imageL).attr("file_hash", fileHash);
        $(imageL).attr("class", $(image).attr("class"));
        imageL.onload = function () {
            if ($(this).hasClass("imageViewDetail")) {
                var fileHash = $(this).attr("file_hash");
                if (fileHash == getUrlHashKeys().file_hash) {
                    mainImage.src = this.src;
                }
            }
            else {
                mainImage.src = this.src;
            }
        }
        imageL.src = "../api/myphotos/image?file_hash=" + fileHash + "&t=l&changeset_no=" + changeset;
    }
    blockImageDownload();
}

function showProfile() {
    $.get("../api/user/me", function (response) {
        window.__userData = response.data;
        if (window.__userData == undefined) {
            signIn();
        }
        else {
            window.location = "./userprofile.z"
        }
    });
};

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

    function getUserInfo(){
        $.get("../api/user/me", function (response) {
            window.__userData = response.data;
         
        });
    }
    getUserInfo();
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


    $("#btnAddToHomeScreen").click(function(){
    
        deferredPrompt.prompt();
        // Wait for the user to respond to the prompt
        deferredPrompt.userChoice
          .then((choiceResult) => {
              if (choiceResult.outcome === 'accepted') {
              console.log('User accepted the A2HS prompt');
            } else {
                console.log('User dismissed the A2HS prompt');
           }
            deferredPrompt = null;
            });
    });


    $('#imageDetailView').on('hide.bs.modal', function (e) {
        //$('#productViewDetails').html("Loading...");
        //window.location.hash = "pv=hide";
        $('body').removeClass("bodyScrollBlock");
    });

    $('#imageDetailView').on('show.bs.modal', function (e) {
        $('#imageDetailView').find(".modal-content").css("width", $(window).width() + "px");
        $('#imageDetailView').find(".modal-content").css("height", $(window).height() + "px");
        //var productKey = $('#productDetailView').attr("product_key")
       //$('#productViewDetails').load("./product_details.z?product_key=" + productKey);
        $('body').addClass("bodyScrollBlock");

    });

});




function getUrlHashKeys(){
    var keys = {};
    window.location.hash.replace("#!#", "").split("&").forEach(function (d) {
        var keyVal = d.split("=");
        keys[keyVal[0]] = keyVal[1];
    });
    return keys;
}

var prevScrollpos = window.pageYOffset;
window.onscroll = function () {
    var currentScrollPos = window.pageYOffset;
    if (prevScrollpos > currentScrollPos || currentScrollPos < 100) {
        showToolbars();
    } else {

        hideToolbars();
    }
    prevScrollpos = currentScrollPos;
}


function showToolbars() { 
    if (document.getElementById("navbar") != null) {
        document.getElementById("navbar").style.top = "0";
    }
    if (document.getElementById("navbarfooter") != null) {
        document.getElementById("navbarfooter").style.bottom = "0";
    }
}

function hideToolbars() {
    if(document.getElementById("navbar")!=null){
        document.getElementById("navbar").style.top = "-49px";
    }
    if(document.getElementById("navbarfooter")!=null)
    {
        document.getElementById("navbarfooter").style.bottom = "-49px";
    }
}
function blockImageDownload() {
    $('img').mousedown(function (e) {
        if (e.button == 2) { // right click
            return false; // do nothing!
        }
    });
}

function signOut() {
    removeCacheKey("/index.z", function () {
        window.location = "../signup/logout.z?rurl=../galleryv2/index.z";
    });

}
function signIn() {
    removeCacheKey("/index.z", function () {
        window.location = "../auth/sociallogin.html?rurl=../galleryv2/index.z";
    });
}

function updateRURL() {
    $('a').each(function () {
        if ($(this).attr("rurl") != undefined) {
            $(this).attr("href", $(this).attr("href") + "?rurl=" + encodeURIComponent($(this).attr("rurl")))
            console.log($(this).attr("href"));
        }
    });
}
updateRURL();