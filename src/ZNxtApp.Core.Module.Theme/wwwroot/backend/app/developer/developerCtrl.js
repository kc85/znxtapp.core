(function () {
    
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.developerCtrl', ['$scope', '$location', '$rootScope', 'dataService', 'userData','loggerService',
    function ($scope, $location, $rootScope, dataService, userData, logger) {
       
        $scope.name = "developer001";

        $scope.showError = function () {
            logger.error("This is test error ");
        };
        $scope.showDebug= function () {
            logger.debug("This is test Debug ");
        };
        $scope.showSuccess= function () {
            logger.success("This is test success");
        };
    }]);
})();