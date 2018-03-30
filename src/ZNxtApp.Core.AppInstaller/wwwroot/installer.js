$(document).ready(function () {
    getStatus();

    $("#btnInstall").click(function () {
        prerequisiteCheck(function () {
            $("#btnInstall").html("Installing...");
            startInstall();
        }, function (data) {
            console.log(data);
            alert("prerequisiteCheck error");
        }
        );
    });

    function getStatus() {
        var jqxhr = $.get("./install/checkstatus", function (data) {
            console.log("success", data);
            if (!data.is_prerequisite_check) {
                prerequisiteCheck();
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
        installData.AdminAccount = "khanin@znxtapp.com";
        installData.AdminPassword = "password";
        installData.Name = "ZNxtApp";
        installData.InstallType = 0;
        installData.DefaultModules = ["ZNxtApp.Core.Module.App/1.0.1-Alpha","ZNxtApp.Core.Module.Theme/1.0.6-Alpha"];

        $.post("./install/start", JSON.stringify(installData),
            function (data, status) {
                console.log(data, status);
                window.location = "./installcomplete.z";
            });
    }
});