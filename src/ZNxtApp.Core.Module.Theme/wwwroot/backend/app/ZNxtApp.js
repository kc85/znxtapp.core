var __ZNxtAppName = 'ZNxtAppAdmin';

(function () {
   
    var ZApp = angular.module(__ZNxtAppName, ['ngAnimate']);
    var isloaded = false;
   
    ZApp.controller(__ZNxtAppName + '.Main', ['$scope', '$location', '$rootScope', 'dataService', 'userData','$timeout',
        function ($scope, $location, $rootScope, dataService, userData, $timeout) {
            $scope.appName = __ZNxtAppName;
            $scope.user = userData;
            $scope.showError = false;
            $scope.showSuccess = false;
            $scope.showDebug = false;
            $scope.showProgress = false;

            $scope.errorMesage = "Something worng in the server";
            $scope.successMesage = "";
            $scope.debugMesage = "";
            
            $scope.errorTimeout = undefined;
            $scope.debugTimeout = undefined;
            $scope.successTimeout = undefined;
            $scope.showProgressTimeout = undefined;

            $scope.$on("onHttpStart", function (e, data) {
                if ($scope.showProgressTimeout != undefined) {
                    $timeout.cancel($scope.showProgressTimeout);
                }
                $scope.showProgressTimeout = $timeout(function () {
                    $scope.showProgress = true;
                }, 300);
               
            });
            $scope.$on("onHttpEnd", function (e, data) {
                if ($scope.showProgressTimeout != undefined) {
                    $timeout.cancel($scope.showProgressTimeout);
                }
                $scope.showProgressTimeout = undefined;
                $scope.showProgress = false;
            });
            $scope.$on("onError", function (e, data) {
                $scope.showError = true;
                if ($scope.errorTimeout != undefined) {
                    $timeout.cancel($scope.errorTimeout);
                    $scope.errorMesage = $scope.errorMesage  + ". " + data.text;
                }
                else {
                    $scope.errorMesage =  data.text;
                }
                $scope.errorTimeout  =  $timeout(function () {
                    $scope.closeError();
                }, data.timeout);
            });

            $scope.$on("onSuccess", function (e, data) {
                $scope.showSuccess = true;
                if ($scope.successTimeout != undefined) {
                    $timeout.cancel($scope.successTimeout);
                    $scope.successMesage =$scope.successMesage+". "+ data.text;
                }
                else {
                    $scope.successMesage = data.text;
                }

                $scope.successTimeout = $timeout(function () {
                    $scope.closeSuccess();
                }, data.timeout);

            });
            $scope.$on("onDebug", function (e, data) {
                $scope.showDebug = true;
               
                if ($scope.debugTimeout != undefined) {
                    $timeout.cancel($scope.debugTimeout);
                    $scope.debugMesage = $scope.debugMesage + ". " + data.text;
                }
                else {
                    $scope.debugMesage = data.text;
                }
                $scope.debugTimeout = $timeout(function () {
                    $scope.closeDebug();
                }, data.timeout);

            });
            $scope.closeError = function () {
                if ($scope.errorTimeout != undefined) {
                    $timeout.cancel($scope.errorTimeout);
                    $scope.errorTimeout = undefined;
                }
                $scope.showError = false;
            }
            $scope.closeSuccess = function () {
                if ($scope.successTimeout != undefined) {
                    $timeout.cancel($scope.successTimeout);
                    $scope.successTimeout = undefined;
                }
                $scope.showSuccess = false;
            }
            $scope.closeDebug = function () {
                if ($scope.debugTimeout != undefined) {
                    $timeout.cancel($scope.debugTimeout);
                    $scope.debugTimeout = undefined;
                }
                $scope.showDebug = false;
            }
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

    ZApp.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('LoadingInterceptor');
    }]);


    ZApp.service('LoadingInterceptor',
    ['$q', '$rootScope',
    function ($q, $rootScope, logger) {
        'use strict';

        return {
            request: function (config) {
                $rootScope.$broadcast('onHttpStart');
                return config;
            },
            requestError: function (rejection) {
                $rootScope.$broadcast('onError', { text: "Something wrong in the request", timeout: 2000 });
                $rootScope.$broadcast('onHttpEnd');
                return $q.reject(rejection);
            },
            response: function (response) {
                $rootScope.$broadcast('onHttpEnd');
                if (response.data != undefined && response.data.code != undefined) {
                    if (response.data.code == 401) {
                        window.location.reload();
                    }
                    else if (response.data.code != 1) {
                        $rootScope.$broadcast('onError', { text: "Something wrong in the request", timeout: 2000 });
                        console.log(response);
                    }
                    
                }
                return response;
            },
            responseError: function (rejection) {
                $rootScope.$broadcast('onHttpEnd');
                $rootScope.$broadcast('onError', { text: "Something wrong in the server", timeout: 2000 });
                return $q.reject(rejection);
            }
        };
    }]);


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