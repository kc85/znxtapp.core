(function () {
    
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.defaultHomeCtrl', ['$scope', '$location', '$rootScope', 'dataService', 'userData',
    function ($scope, $location, $rootScope, dataService, userData) {
       
        $scope.name = "Home 001 ";
    }]);
})();