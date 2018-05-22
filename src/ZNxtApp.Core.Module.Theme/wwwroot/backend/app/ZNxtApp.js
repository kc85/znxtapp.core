var __ZNxtAppName = 'ZNxtAppAdmin';

(function () {
   
    var ZApp = angular.module(__ZNxtAppName, []);
    var isloaded = false;
   
    ZApp.controller(__ZNxtAppName + '.Main', ['$scope', '$location', '$rootScope', 'dataService', 'userData',
        function ($scope, $location, $rootScope, dataService, userData) {
            $scope.appName = __ZNxtAppName;
            $scope.user = userData;
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

    ZApp.constant('menus', __menus);
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