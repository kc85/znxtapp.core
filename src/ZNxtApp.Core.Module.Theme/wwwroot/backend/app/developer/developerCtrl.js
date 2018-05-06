(function () {
    
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.developerCtrl', ['$scope', '$location', '$rootScope', 'dataService', 'userData',
    function ($scope, $location, $rootScope, dataService, userData) {
       
        $scope.name = "developer001";
    }]);
})();