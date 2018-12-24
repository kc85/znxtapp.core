(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.logDetailsCtrl', ['$scope', '$controller', '$location', '$rootScope', '$window', 'dataService', 'userData',
    function ($scope, $controller, $location, $rootScope, $window, dataService, userData) {
        $scope.log = {};
        var scrollX = 0;
        var scrollY;
        $scope.closeDetails = function () {
            $scope.$emit("onHideLogViewDetails", $scope.log);
            $window.scrollTo(scrollX, scrollY);
        }
        $scope.$on("onShowLogViewDetails", function (e, log) {
            $scope.log = log;
            scrollY = $window.scrollY;
            scrollX = $window.scrollX;
            $window.scrollTo(0, 0);
        });
    }]);
})();