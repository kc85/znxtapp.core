(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.pagesCtrl', ['$scope', '$location', '$rootScope', '$controller', 'dataService', 'userData',
    function ($scope, $location, $rootScope, $controller, dataService, userData) {
        angular.extend(this, $controller(__ZNxtAppName + '.gridBaseCtrl', { $scope: $scope }));
        $scope.name = "Pages";
        $scope.pageData = {};
        $scope.showDetails = false;
        $scope.filterIncludeColumns = ["id", "file_path", "module_name", "content_type"];

        $scope.active = function () {
            if ($scope.loadingData == false) {
                $scope.loadingData = true;
                dataService.get("./api/admin/common/get?collection=wwwroot&pagesize=" + $scope.pageSize + "&fields=id,content_type,file_path,module_name,override_by,comment,is_override&currentpage=" + $scope.currentPage + "&filter=" + $scope.getFilter()).then(function (response) {
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
            $scope.$broadcast("onShowPageViewDetails", data);
        }
        $scope.$on("onHidePageViewDetails", function (data) {
            $scope.showDetails = false;
        });
        $scope.pageNumberChanged = function () {
            $scope.gotoPage($scope.currentPageShow);
        };
        $scope.active();
    }]);
})();