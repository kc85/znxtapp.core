(function () {
    'use strict';
    var ZApp = angular.module(__ZNxtAppName);
    ZApp.factory('loggerService', ['$http', '$rootScope', loggerService]);

    function loggerService($http, $rootScope) {
        var service = {
            error: callError,
            success: callSuccess,
            debug: callDebug
        };
        return service;
        function callError(message) {
            $rootScope.$broadcast('onError', { text: message, timeout: 5000 });
        };
        function callSuccess(message) {
            $rootScope.$broadcast('onSuccess', { text: message, timeout: 3000 });
        };
        function callDebug(message) {
            $rootScope.$broadcast('onDebug', { text: message, timeout: 2000 });
        };
    }
})();
