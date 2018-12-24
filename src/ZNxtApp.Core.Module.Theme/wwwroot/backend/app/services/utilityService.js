(function () {
    'use strict';
    var ZApp = angular.module(__ZNxtAppName);
    ZApp.factory('utilityService', [utilityService]);

    function utilityService() {
        var service = {
            getTimeStamp: getTimeSpampData
        };
        return service;
        function getTimeSpampData() {
            return new Date.toString();
        };
    }
})();