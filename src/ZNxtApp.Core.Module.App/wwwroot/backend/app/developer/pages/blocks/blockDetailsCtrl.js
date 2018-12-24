(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.blockDetailsCtrl', ['$scope', '$controller', '$location', '$rootScope', '$window', 'dataService', 'userData', 'loggerService',
    function ($scope, $controller, $location, $rootScope, $window, dataService, userData, logger) {
        $scope.block = {};
        var scrollX = 0;
        var scrollY;

        $scope.closeDetails = function () {
            $scope.$emit("onHideBlockViewDetails", $scope.block);
            $window.scrollTo(scrollX, scrollY);
        }
        $scope.$on("onShowBlockViewDetails", function (e, block) {
            $scope.block = block;
            scrollY = $window.scrollY;
            scrollX = $window.scrollX;
            $window.scrollTo(0, 0);
        });
    }]);
})();