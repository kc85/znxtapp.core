(function () {
    'use strict';
    var ZApp = angular.module(__ZNxtAppName);
    ZApp.factory('dataService', ['$http', datacontext]);

    function datacontext($http) {
        var service = {
            get: getApiCall,
            post: postApiCall
        };
        return service;
        function getApiCall(url) {
            return $http.get(url);
        };
        function postApiCall(url, data) {
            return $http.post(url, data);
        };
      
    }
})();
