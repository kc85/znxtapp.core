$(document).ready(function () {
    getStatus();
    var defaultModules = [
        "ZNxtApp.Core.Module.App/1.0.11.38907-Beta",
        "ZNxtApp.Core.Module.Theme/1.0.11.38791-Beta",
        "ZNxtApp.Core.Module.Theme.Frontend/1.0.11.38791-Beta",
        "ZNxtApp.Core.Module.SMS.TextLocal/1.0.11.38791-Beta"
    ];

    $("#btnInstall").click(function () {
        prerequisiteCheck(function () {
            $("#wizardProfile").hide();
            $("#wizardInprogress").fadeIn();
            $("#btnInstall").html("Installing...");
            startInstall();
        }, function (data) {
            console.log(data);
            alert("prerequisiteCheck error");
        }
        );
    });

    $("#btnNext").click(function () {
        $("#adminEamilShow").html($("#adminEmail").val());
    });

    function getStatus() {
        var jqxhr = $.get("./install/checkstatus", function (data) {
            console.log("success", data);
            if (!data.is_prerequisite_check) {
                prerequisiteCheck();
            }
            if (data.status == "Inprogress") {
                $("#wizardProfile").hide();
                $("#wizardInprogress").fadeIn();
                setTimeout(function () { getStatus(); }, 2000);
            }
            else if (data.status == "Finish") {
                window.location = "./installcomplete.z";
            }
            else {
                $("#wizardProfile").show();
            }
        })
        .done(function () {
        })
        .fail(function () {
            window.location.reload();
        })
        .always(function () {
        });
    };

    function prerequisiteCheck(callback, errorCallback) {
        var jqxhr = $.get("./install/checkprerequisite", function (data) {
            console.log("success", data);

            var success = true;
            data.data.forEach(function (f) {
                if (!f.status) {
                    success = false;
                }
            });
            if (callback != undefined && success) {
                callback(data.data);
            }
            else if (errorCallback != undefined) {
                errorCallback(data.data);
            }
        })
        .done(function () {
        })
        .fail(function () {
            console.log("error");
        })
        .always(function () {
        });
    };

    function startInstall() {
        var installData = {};
        installData.AdminAccount = $("#adminEmail").val();
        installData.AdminPassword = $("#adminPassword").val();
        installData.Name = "ZNxtApp";
        installData.InstallType = 0;
        installData.DefaultModules = defaultModules;

        $.post("./install/start", JSON.stringify(installData),
            function (data, status) {
                console.log(data, status);
                window.location = "./installcomplete.z";
            });
    }
});