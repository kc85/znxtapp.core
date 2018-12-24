(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.serverRoutesCtrl', ['$scope', '$location', '$rootScope', '$controller', 'dataService', 'userData',
    function ($scope, $location, $rootScope, $controller, dataService, userData) {
        angular.extend(this, $controller(__ZNxtAppName + '.gridBaseCtrl', { $scope: $scope }));
        $scope.name = "Server Routes";
        $scope.pageData = {};
        $scope.showDetails = false;
        $scope.filterIncludeColumns = ["key", "Route", "ExecultAssembly", "ExecuteType", "module_name"];

        $scope.active = function () {
            if ($scope.loadingData == false) {
                $scope.loadingData = true;
                dataService.get("./api/admin/server_routes?pagesize=" + $scope.pageSize + "&currentpage=" + $scope.currentPage + "&filter=" + $scope.getFilter()).then(function (response) {
                    if (response.data.code == 1) {
                        $scope.currentPageShow = $scope.currentPage;
                        $scope.pageData = response.data;
                    }
                    $scope.loadingData = false;
                });
            }
        }

        $scope.showDetailsPage = function (data) {
            $scope.showDetails = true;
            $scope.$broadcast("onShowSettingViewDetails", data);
        }
        $scope.$on("onHideSettingViewDetails", function (data) {
            $scope.showDetails = false;
        });
        $scope.pageNumberChanged = function () {
            $scope.gotoPage($scope.currentPageShow);
        };
        $scope.active();
    }]);
})();