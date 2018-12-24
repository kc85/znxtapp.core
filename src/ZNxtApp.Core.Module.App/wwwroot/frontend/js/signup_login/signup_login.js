var success_code = 1;
function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}
var redirectUrl = GetParameterValues("rurl");
if (redirectUrl == undefined) {
    redirectUrl = appRootPath + "/index.z";
}
function validateEmail($email) {
    if ($email.length == 0) {
        return false;
    }
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test($email);
}

function validatePhone(field) {
    if (field.length == 0) {
        return false;
    }
    if (field.match(/^\d{10}/)) {
        return true;
    }

    return false;
}

// facebook login

// This is called with the results from from FB.getLoginStatus().
function statusChangeCallback(response) {
    // The response object is returned with a status field that lets the
    // app know the current login status of the person.
    // Full docs on the response object can be found in the documentation
    // for FB.getLoginStatus().
    if (response.status === 'connected') {
        // Logged into your app and Facebook.
        if (facebooksuccesscallback != undefined) {
            facebooksuccesscallback(response);
        }
    } else {
        // The person is not logged into your app or we are unable to tell.
        // $("#errorMessage").html('Please log into this app.');
        // $("#errorMessage").show();
    }
}

// This function is called when someone finishes with the Login
// Button.  See the onlogin handler attached to it in the sample
// code below.
function checkLoginState() {
    FB.getLoginStatus(function (response) {
        statusChangeCallback(response);
    });
}

window.fbAsyncInit = function () {
    FB.init({
        appId: _fbapp_id,
        cookie: true,  // enable cookies to allow the server to access
        // the session
        xfbml: true,  // parse social plugins on this page
        version: 'v2.8' // use graph api version 2.8
    });

    // Now that we've initialized the JavaScript SDK, we call
    // FB.getLoginStatus().  This function gets the state of the
    // person visiting this page and can return one of three states to
    // the callback you provide.  They can be:
    //
    // 1. Logged into your app ('connected')
    // 2. Logged into Facebook, but not your app ('not_authorized')
    // 3. Not logged into Facebook and can't tell if they are logged into
    //    your app or not.
    //
    // These three cases are handled in the callback function.

    //FB.getLoginStatus(function (response) {
    //    statusChangeCallback(response);
    // });
};

// Load the SDK asynchronously
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

var facebooksuccesscallback = function (fbresponse) {
    console.log(fbresponse);
    $(".login").fadeOut();
    $("#divRedirecting").show();

    $.ajax({
        url: appRootPath + "/api/user/facebook/auth",
        type: 'post',
        data: '{"auth_token": "' + fbresponse.authResponse.accessToken + '", "rurl" : "' + redirectUrl + '"}',
        contentType: "application/json",
        dataType: 'json',
        success: function (data) {
            if (data.code === success_code) {
                $(".login").fadeOut();
                $("#divRedirecting").show();

                window.location = redirectUrl;
            }
            else {
                capchaExpiredCallback();
                animating = false;
                $(".login").show();
                $("#divRedirecting").hide();
                $("#btnSingin").html("Sign in");
                $("#errorMessage").show();
                $("#errorMessage").html(data.message);
            }
        },
        error: function (err) {
            console.log(err);
            grecaptcha.reset();
            $("#btnSingin").html("Sign in");

            $(that).removeClass("success processing");
            $("#errorMessage").show();
            $("#errorMessage").html("Auth fail<br/>");
            animating = false;
        }
    });

    //FB.api('/me', function (response) {
    //    console.log(response);
    //});
}