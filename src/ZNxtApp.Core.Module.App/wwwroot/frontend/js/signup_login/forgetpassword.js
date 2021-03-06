var grecaptcharesponse = undefined;
var otp_security_token = "";
var user_type = "Email";
var imNotARobot = function (val) {
    grecaptcharesponse = val;
    $("#txtMobileNumber").focus();
    $("#gmessage").hide();
    $(".login__form").show();
};
$(document).ready(function () {
    var redirectUrl = GetParameterValues("rurl");
    if (redirectUrl == undefined) {
        redirectUrl = appRootPath + "/index.z";
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

    $("#btnSendForgetOTP").click(function (e) {
        $("#errorMessage").hide();
        if (animating) return;

        var that = this;
        ripple($(that), e);

        var mobileNo = $("#txtMobileNumber").val()
        if (!validateEmail(mobileNo) && !validate(mobileNo)) {
            $("#errorMessage").show();
            $("#errorMessage").html("Enter valid email id or phone");
            return;
        }
        animating = true;
        $(that).addClass("processing");
        $.ajax({
            url: appRootPath + "/api/user/forgetpassword/SendOTP",
            type: 'post',
            data: '{"user_id": "' + mobileNo + '", "g-recaptcha-response" : "' + grecaptcharesponse + '", "user_type" : "' + user_type + '"}',
            contentType: "application/json",
            dataType: 'json',
            success: function (data) {
                console.log(data);
                animating = false;
                $(that).removeClass("success processing");

                if (data.code === success_code) {
                    otp_security_token = data.security_token;
                    $("#txtMobileNumber").attr("disabled", "true");
                    $("#btnSendForgetOTP").hide();
                    $(".g_captche").hide();
                    $("#divOtp").show();
                    $("#divPassword").show();
                }
                else {
                    grecaptcha.reset();
                    $("#errorMessage").show();
                    $("#errorMessage").html(data.message);
                }
            },
            error: function (err) {
                console.log(err);
                animating = false;
                $("#btnSendForgetOTP").hide();
                $(that).removeClass("success processing");
                $("#errorMessage").show();
                $("#errorMessage").html("Server Error, Please report error to site admin");
                grecaptcha.reset();
            }
        });
    });

    //$("#btnSignupOTPSubmit").click(function (e) {
    //    $("#errorMessage").hide();
    //    if (animating) return;

    //    var that = this;
    //    ripple($(that), e);

    //    var mobileNo = $("#txtMobileNumber").val();
    //    if (!validateEmail(mobileNo) && !validate(mobileNo)) {
    //        $("#errorMessage").show();
    //        $("#errorMessage").html("Enter valid email id or phone");
    //        return;
    //    }
    //    animating = true;
    //    $(that).addClass("processing");
    //    var otp = $("#txtOTP").val();

    //    $.ajax({
    //        url: appRootPath+ "/api/user/registration/OTP",
    //        type: 'post',
    //        data: '{"otp": "' + otp + '", "user_id" :"' + mobileNo + '", "security_token" :"' + otp_security_token + '","user_type" : "' + user_type + '"}',
    //        contentType: "application/json",
    //        dataType: 'json',
    //        success: function (data) {
    //            animating = false;
    //            $(that).removeClass("success processing");
    //            if (data.code === success_code) {
    //                $("#divOtp").fadeOut();
    //                $("#divPassword").fadeIn();
    //               // window.location = appRootPath+ data.rurl;
    //            }
    //            else {
    //                $("#errorMessage").show();
    //                $("#errorMessage").html(data.message);
    //            }
    //        },
    //        error: function (err) {
    //            console.log(err);
    //            animating = false;
    //            $(that).removeClass("success processing");
    //            $("#errorMessage").show();
    //            $("#errorMessage").html("Unauthorized :( <br/>");
    //        }
    //    });
    //});

    $("#btnResetPassword").click(function (e) {
        $("#errorMessage").hide();
        if (animating) return;

        var that = this;
        ripple($(that), e);

        var email = $("#txtMobileNumber").val();
        if (!validateEmail(email) && !validate(email)) {
            $("#errorMessage").show();
            $("#errorMessage").html("Enter valid email id or phone");
            return;
        }
        var password = $("#txtPassword").val();
        var passwordC = $("#txtConfirmPassword").val();
        var otp = $("#txtOTP").val();
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
            url: appRootPath + "/api/user/forgetpassword/resetpassword",
            type: 'post',
            data: '{"user_id": "' + email + '", "password" :"' + password + '", "confirm_password":"' + passwordC + '", "g-recaptcha-response" : "' + grecaptcharesponse + '","user_type" : "' + user_type + '","otp" : "' + otp + '"}',
            contentType: "application/json",
            dataType: 'json',
            success: function (data) {
                if (data.code === success_code) {
                    $("#divOtp").fadeOut();
                    $("#divPassword").fadeIn();
                    window.location = appRootPath + data.rurl;
                }
                else {
                    animating = false;
                    $(that).removeClass("success processing");
                    $("#errorMessage").show();
                    $("#errorMessage").html(data.message);
                }
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
            $("#divSendOtp").fadeIn();
            $("#divFbSignUp").fadeOut();
            $("#errorMessage").fadeOut();

            user_type = "Email";
            $("#btnSendForgetOTP").html("Send OTP on My Email");
        }
        else if (validatePhone($(txt).val())) {
            $("#divSendOtp").fadeIn();
            $("#divFbSignUp").fadeOut();
            $("#errorMessage").fadeOut();

            user_type = "PhoneNumber";
            $("#btnSendForgetOTP").html("Send OTP on My Mobile");
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