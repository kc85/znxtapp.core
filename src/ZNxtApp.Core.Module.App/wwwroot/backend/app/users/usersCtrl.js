(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.usersCtrl', ['$scope', '$controller', '$location', '$rootScope', 'dataService', 'userData',
    function ($scope,$controller, $location, $rootScope, dataService, userData) {
        angular.extend(this, $controller(__ZNxtAppName + '.gridBaseCtrl', { $scope: $scope }));
        $scope.name = "Users";
        $scope.pageData = {};        
        $scope.filterIncludeColumns = ["id", "name", "email"];
        $scope.showDetails = false;
        $scope.loadingData = false;
        $scope.active = function() {
            fetchUserInfo();
        }

        function fetchUserInfo() {
            if ($scope.loadingData == false) {
                $scope.loadingData = true;
                dataService.get("./api/admin/users?pagesize=" + $scope.pageSize + "&currentpage=" + $scope.currentPage + "&filter=" + $scope.getFilter()).then(function (response) {
                    if (response.data.code == 1) {
                        $scope.currentPageShow = $scope.currentPage;
                        $scope.pageData = response.data;
                        console.log($scope.pageData);
                    }
                    $scope.loadingData = false;
                });
            }
        }
        
        $scope.showDetailsPage = function (data) {
            $scope.showDetails = true;            
            $scope.$broadcast("onShowUserDetails", data);
        }

        $scope.$on("onHideUserDetails", function (log) {
            $scope.showDetails = false;
        });

        $scope.pageNumberChanged = function () {
            $scope.gotoPage($scope.currentPageShow);
        };

        $scope.active();

        $scope.$on("onUserInfoUpdate", function () {
            $scope.active();
        });
    }]);
})();