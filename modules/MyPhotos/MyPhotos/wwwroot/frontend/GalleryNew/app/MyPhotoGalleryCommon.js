
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
    blockImageDownload();
}

function showProfile() {
    $.get("../api/user/me", function (response) {
        window.__userData = response.data;
        if (window.__userData == undefined) {
            showLogin();
        }
        else {
            window.location = "./userprofile.z"
        }
    });
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

function googleLoginCheck(googleUser) {
    // Useful data for your client-side scripts:
    var profile = googleUser.getBasicProfile();
    var id_token = googleUser.getAuthResponse().id_token;
    $.ajax({
        url:  "../api/user/google/auth",
        type: 'post',
        data: '{"auth_token": "' + id_token+ '", "rurl" : "./"}',
        contentType: "application/json",
        dataType: 'json',
        success: function (data) {
            if($('#myphotoLogin').css("display")=='block'){
                removeCacheKey("/api/myphotos/gallery");
                removeCacheKey("/indexnew.z",function(){
                    window.location.reload();
                });
            }
            $.get("../api/user/me", function (response) {
                window.__userData = response.data;
                if(window.__userData!=undefined){
                    setCookie(userlogincookiekey,true,30);
                }
            });
        },
        error: function (err) {
            console.log(err);
            console.log("getUserLogin Cookie", getCookie(userlogincookiekey));
            window.__userData = undefined;
        }
    });

};

function initGoogleAuth(callback){
    gapi.load('auth2', function() {
        gapi.auth2.init().then(function(){ 
            if(callback!=undefined){
                callback();
            }
        });
    });
}
function googleSignOut() {
    initGoogleAuth(function () {
        var auth2 = gapi.auth2.getAuthInstance();
        auth2.signOut().then(function () {
            setCookie(userlogincookiekey, false, 30);
            window.location = "../signup/logout.z?rurl=../gallerynew/indexnew.z";
        });
    });
}


$(document).ready(function () {

    function getUserInfo(){
        $.get("../api/user/me", function (response) {
            window.__userData = response.data;
            if(window.__userData!=undefined){
                setCookie(userlogincookiekey,true,30);
            }
            else{
                var c=  getCookie(userlogincookiekey);
                setCookie(userlogincookiekey,false,30);
                if(c == "true"){
                    googleSignOut();
                }
            }
        });
    }
    getUserInfo();
    //loadGalleryLargeImage();
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

});


function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays*24*60*60*1000));
    var expires = "expires="+ d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for(var i = 0; i <ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

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
        document.getElementById("navbar").style.top = "0";
        document.getElementById("navbarfooter").style.bottom = "0";
    } else {

        document.getElementById("navbar").style.top = "-49px";
        document.getElementById("navbarfooter").style.bottom = "-49px";
    }
    prevScrollpos = currentScrollPos;
}


function blockImageDownload() {
    $('img').mousedown(function (e) {
        if (e.button == 2) { // right click
            return false; // do nothing!
        }
    });
}