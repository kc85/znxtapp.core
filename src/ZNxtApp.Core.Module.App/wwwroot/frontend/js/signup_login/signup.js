var grecaptcharesponse = undefined;
var success_code = 1;
var imNotARobot = function (val) {
    grecaptcharesponse = val;
    $("#txtMobileNumber").focus();
    $("#gmessage").hide();
    $(".login__form").show();
};
$(document).ready(function () {
    var redirectUrl = GetParameterValues("rurl");
    if (redirectUrl == undefined) {
        redirectUrl = "./index.html";
    }

    var animating = false,
        submitPhase1 = 1100,
        submitPhase2 = 400,
        logoutPhase1 = 800,
        $login = $(".login"),
        $app = $(".app");

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
                console.log(data);
                window.location = data.data.facebook_graph_api_url;
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

   $("#btnSendSignUpOTP").click(function (e) {
        $("#errorMessage").hide();
        if (animating) return;

        var that = this;
        ripple($(that), e);

        var mobileNo = $("#txtMobileNumber").val()
        if (!validate(mobileNo)) {
            $("#errorMessage").show();
            $("#errorMessage").html("Enter 10 digit mobile number");
            return;
        }
        animating = true;
        $(that).addClass("processing");
        $.ajax({
            url: "./api/user/registration/SendOTP",
            type: 'post',
            data: '{"phone": "' + mobileNo + '", "g-recaptcha-response" : "' + grecaptcharesponse + '"}',
            contentType: "application/json",
            dataType: 'json',
            success: function (data) {
                console.log(data);
                animating = false;
                $(that).removeClass("success processing");

                if (data.code === success_code) {
                    
                    $("#txtMobileNumber").attr("disabled", "true");
                    $("#btnSendSignUpOTP").hide();
                    $(".g_captche").hide();
                    $("#divOtp").show();
                }
                else {
                    $("#errorMessage").show();
                    $("#errorMessage").html(data.message);
                }
            },
            error: function (err) {
                console.log(err);
                animating = false;
                $("#btnSendSignUpOTP").hide();
                $(that).removeClass("success processing");
                $("#errorMessage").show();
                $("#errorMessage").html("Server Error, Please report error to site admin");
                grecaptcha.reset();
            }
        });
    });

    $("#btnSignupOTPSubmit").click(function (e) {
        $("#errorMessage").hide();
        if (animating) return;

        var that = this;
        ripple($(that), e);

        var mobileNo = $("#txtMobileNumber").val();
        if (!validate(mobileNo)) {
            $("#errorMessage").show();
            $("#errorMessage").html("Enter 10 digit mobile number");
            return;
        }
        animating = true;
        $(that).addClass("processing");
        var otp = $("#txtOTP").val();

        $.ajax({
            url: "./api/user/registration/OTP",
            type: 'post',
            data: '{"otp": "' + otp + '", "phone" :"' + mobileNo + '"}',
            contentType: "application/json",
            dataType: 'json',
            success: function (data) {
                window.location = "./setpassword.html";
            },
            error: function (err) {
                console.log(err);
                animating = false;
                $(that).removeClass("success processing");
                $("#errorMessage").show();
                $("#errorMessage").html("Unauthorized :( <br/>");
            }
        });
    });


    $("#btnSignupWithPassSubmit").click(function (e) {

        $("#errorMessage").hide();
        if (animating) return;

        var that = this;
        ripple($(that), e);

        var email = $("#txtMobileNumber").val();
        if (!validateEmail(email)) {
            $("#errorMessage").show();
            $("#errorMessage").html("Enter valid email id ");
            return;
        }
        var password = $("#txtPassword").val();
        var passwordC = $("#txtConfirmPassword").val();

        if (password.length < 5) {
            $("#errorMessage").show();
            $("#errorMessage").html("Password length should be min of 5 chars");
            return;
        }
        if (password != passwordC) {
            $("#errorMessage").show();
            $("#errorMessage").html("Password and confirm password not match");
            return;
        }
      
        $.ajax({
            url: "./api/user/registration/username",
            type: 'post',
            data: '{"email": "' + email + '", "password" :"' + password + '", "g-recaptcha-response" : "' + grecaptcharesponse + '"}',
            contentType: "application/json",
            dataType: 'json',
            success: function (data) {
                animating = false;
                window.location = "./index.html";
            },
            error: function (err) {
                console.log(err);
                animating = false;
                $(that).removeClass("success processing");
                $("#errorMessage").show();
                $("#errorMessage").html("Server Error, Please report error to site admin");
                grecaptcha.reset();
            }
        });


    });


    function validate(val) {
        var pattern = new RegExp("^[0-9]{10}$");

        if (pattern.test(val)) {
            return true;
        }
        else {
            return false;
        }
    }

    function GetParameterValues(param) {
        var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < url.length; i++) {
            var urlparam = url[i].split('=');
            if (urlparam[0] == param) {
                return urlparam[1];
            }
        }
    }
});

function onBlurPasswordText(txt) {

    if ($(txt).val().length < 5) {
        $("#errorMessage").show();
        $("#errorMessage").html("Password length should be min of 5 chars");
    }
}
function onBlurSignUpText(txt) {
    
    if ($(".login__form").is(":visible")) {
        if (validateEmail($(txt).val())) {
            $("#divPassword").fadeIn();
            $("#divFbSignUp").fadeOut();
            $("#errorMessage").fadeOut();

        }
        else if (validatePhone($(txt).val())) {
            $("#divSendOtp").fadeIn();
            $("#divFbSignUp").fadeOut();
            $("#errorMessage").fadeOut();

        }
        else {
            $("#divPassword").fadeOut();
            $("#divSendOtp").fadeOut();
            $("#errorMessage").fadeIn();
            $("#errorMessage").html("Enter valid Phone number or Email id");
            $("#divFbSignUp").fadeIn();
        }
    }
}
