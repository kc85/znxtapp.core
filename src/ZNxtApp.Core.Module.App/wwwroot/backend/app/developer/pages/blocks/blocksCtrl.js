(function () {
    
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.blocksCtrl', ['$scope', '$location', '$rootScope', '$controller', 'dataService', 'userData',
    function ($scope, $location, $rootScope, $controller, dataService, userData) {
       
        angular.extend(this, $controller(__ZNxtAppName + '.gridBaseCtrl', { $scope: $scope }));
        $scope.name = "Blocks";
        $scope.pageData = {};
        $scope.showDetails = false;
        $scope.filterIncludeColumns = ["id", "key", "block_path", "display_area",  "comment"];
        $scope.includeFields = ["id", "key", "block_path", "pages", "display_area", "index", "is_enabled", "active_from_date_time", "active_to_date_time", "module_name", "override_by", "is_override", "comment"];
        $scope.active = function () {
            if ($scope.loadingData == false) {
                $scope.loadingData = true;
                dataService.get("./api/admin/common/get?collection=zblock&pagesize=" + $scope.pageSize + "&fields=" + $scope.includeFields.join(",") + "&currentpage=" + $scope.currentPage + "&filter=" + $scope.getFilter()).then(function (response) {
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
            $scope.$broadcast("onShowBlockViewDetails", data);
        }
        $scope.$on("onHideBlockViewDetails", function (data) {
            $scope.showDetails = false;
        });
        $scope.pageNumberChanged = function () {
            $scope.gotoPage($scope.currentPageShow);
        };
        $scope.active();

    }]);
})();