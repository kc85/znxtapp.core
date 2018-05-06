﻿var __ZNxtAppName = 'ZNxtAppAdmin';

(function () {
   
    var ZApp = angular.module(__ZNxtAppName, []);
    var isloaded = false;
   
    ZApp.controller(__ZNxtAppName + '.Main', ['$scope', '$location', '$rootScope', 'dataService', 'userData', 'routes',
        function ($scope, $location, $rootScope, dataService, userData, routes) {
        $scope.appName = __ZNxtAppName;
        $rootScope.$on('$includeContentLoaded', function () {
            //if (isloaded !== true) {
            //    if (userData == undefined || userData.Code != 200) {
            //      //  window.location = "../login.html?rurl=" + window.location + "&show=login";
            //    }
            //    else {
            //        setTimeout(function () { initmainPage(); }, 10);
            //    }
            //    isloaded = true;
            //}
        });
    }]);

    ZApp.constant('routes', __menus);
    ZApp.constant('userData', __userData);
    //ZApp.constant('appInfo', __app_info);
    //ZApp.constant('appSettings', __app_settings);
    //ZApp.config(['$routeProvider', '$locationProvider', 'routes', routeConfigurator]);
    //function routeConfigurator($routeProvider, $locationProvider, routes) {
    //    routes.forEach(function (r) {
    //        $routeProvider.when(r.url, r.config);
    //    });
    //    $routeProvider.otherwise({ redirectTo: '/' });
    //}
})();