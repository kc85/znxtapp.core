(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.usersCtrl', ['$scope', '$controller', '$location', '$rootScope', 'dataService', 'userData',
    function ($scope,$controller, $location, $rootScope, dataService, userData) {
        angular.extend(this, $controller(__ZNxtAppName + '.gridBaseCtrl', { $scope: $scope }));
        $scope.name = "User Profile";
        $scope.logData = {};
        $scope.pageData = {};
        $scope.showDetails = false;
        $scope.active = function() {
            if ($scope.loadingData == false) {
                $scope.loadingData = true;
                $scope.isError = false;
                dataService.get("./api/admin/log?pagesize=" + $scope.pageSize + "&currentpage=" + $scope.currentPage + "&filter=" + $scope.getFilter()).then(function (response) {
                    if (response.data.code == 1) {
                        $scope.currentPageShow = $scope.currentPage;
                        $scope.pageData = $scope.logData = response.data;
                    }
                    else {
                        console.log(response);
                        $scope.isError = true;
                        $scope.errorMessage = "Something went wrong in the server";
                    }
                    $scope.loadingData = false;
                });
            }
        }
        
        $scope.showDetailsPage = function (log) {
            $scope.showDetails = true;
            $scope.$broadcast("onShowLogViewDetails", log);
        }
        $scope.$on("onHideLogViewDetails", function (log) {
            $scope.showDetails = false;
        });
        $scope.pageNumberChanged = function () {
            $scope.gotoPage($scope.currentPageShow);
        };
        $scope.active();
    }]);
})();