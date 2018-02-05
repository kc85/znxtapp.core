$(document).ready(function () {


    getStatus();

    $("#btnInstall").click(function () {
        startInstall();
    });


    function getStatus() {

        var jqxhr = $.get("/install/checkstatus", function (data) {
            console.log("success", data);
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
        installData.AdminAccount= "khanin@znxtapp.com";
        installData.AdminPassword = "password";
        installData.Name = "ZNxtApp";
        installData.InstallType = 0;


        $.post("/install/start",  JSON.stringify(installData),
            function (data, status) {
            console.log(data , status);
        });

        /*
        $.ajax({
            type: "POST",
            url: "/install/start",
            data: JSON.stringify(installData),
            success: function () {
                console.log("Insall start");
            },
            dataType: "application/json"
        });
        */
    }

});
