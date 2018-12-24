(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.moduleCtrl', ['$scope', '$location', '$rootScope', 'dataService', 'userData',
    function ($scope, $location, $rootScope, dataService, userData) {
        $scope.name = "Module 001 ";
        $scope.modules = [];
        function active() {
            dataService.get("./api/admin/module").then(function (response) {
                $scope.modules = response.data.data;
                console.log($scope.modules);
            });
        }
        $scope.uninstall = function (moduleName) {
            if (confirm("Do you want to uninstall module " + moduleName + "?")) {
                console.log(moduleName);
                dataService.post("./api/module/uninstall?module_name=" + moduleName).then(function () {
                    active();
                })
            }
        }
        active();
    }]);
})();