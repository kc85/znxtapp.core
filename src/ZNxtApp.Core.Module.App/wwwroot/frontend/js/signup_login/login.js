var grecaptcharesponse = undefined;

var animating = false;
      
var capchaExpiredCallback = function () {
    grecaptcha.reset();
};


var imNotARobotCallback = function (val) {
    grecaptcharesponse = val;
    $("#errorMessage").hide();
    $("#btnSingin").html("Processing");
    if (animating) return;
    animating = true;
    var that = $("#btnSingin");
    //ripple($(that), e);
    $(that).addClass("processing");

    var mobileNo = $("#txtUserName").val()
    var password = $("#txtPassword").val()

    $.ajax({
        url: appRootPath + "/api/auth/login",
        type: 'post',
        data: '{"user_id": "' + mobileNo + '", "password" :"' + password + '","g-recaptcha-response" : "' + grecaptcharesponse + '"}',
        contentType: "application/json",
        dataType: 'json',
        success: function (data) {
         
            if (data.code === success_code) {
                $("#btnSingin").html("Redirecting....");
                window.location = redirectUrl;
            }
            else {
                capchaExpiredCallback();
                animating = false;
                $("#btnSingin").html("Sign in");
                $(that).removeClass("success processing");
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
            $("#errorMessage").html("Username or Password incorrect<br/>");
            animating = false;
        }
    });
};
function ripple(elem, e) {
    $(".ripple").remove();
    var elTop = elem.offset().top,
        elLeft = elem.offset().left,
        x = e.pageX - elLeft,
        y = e.pageY - elTop;
    var $ripple = $("<div class='ripple'></div>");
    $ripple.css({ top: y, left: x });
    elem.append($ripple);
};
function onBlur() {
   
        $("#errorMessage").fadeOut();
};


$(document).ready(function () {
    var redirectUrl = GetParameterValues("rurl");
    if (redirectUrl == undefined) {
        redirectUrl = appRootPath + "/index.z";
    }

    $login = $(".login"),
    $app = $(".app");

    $("#btnFBLogin").click(function (e) {
        if (animating) return;
        animating = true;
        var that = this;
        ripple($(that), e);
        $(that).addClass("processing");

        $.ajax({
            type: 'POST',
            url: './api/user/facebook/auth',
            success: function (data) {
                $(".login").hide();
                $("#divRedirecting").show();
                window.location = data['facebook_oauth_url'];
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
                animating = false;
                $(that).removeClass("success processing");
                alert("Ërror")
            },
            contentType: "application/json",
            dataType: 'json'
        });
    });

    $("#btnOTPLogin").click(function () {
        window.location = "./mobilelogin";
    });

});
