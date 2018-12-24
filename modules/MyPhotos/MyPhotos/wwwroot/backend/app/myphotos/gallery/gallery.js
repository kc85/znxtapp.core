(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.myphotos.gallery', ['$scope', '$controller', '$location', '$rootScope', 'dataService', 'userData',
    function ($scope, $controller, $location, $rootScope, dataService, userData) {
        angular.extend(this, $controller(__ZNxtAppName + '.gridBaseCtrl', { $scope: $scope }));
        $scope.name = "Gallery";
        $scope.logData = {};
        $scope.pageData = {};
        $scope.showDetails = false;
        $scope.filterIncludeColumns = ["id", "name", "diaplay_name"];

        $scope.active = function () {
            if ($scope.loadingData == false) {
                $scope.loadingData = true;
                
                dataService.get("./api/admin/myphotos/gallery?pagesize=" + $scope.pageSize + "&currentpage=" + $scope.currentPage + "&filter=" + $scope.getFilter()).then(function (response) {
                    if (response.data.code == 1) {
                        $scope.currentPageShow = $scope.currentPage;
                        $scope.pageData = $scope.logData = response.data;
                    }
                    
                    $scope.loadingData = false;
                });
            }
        }
        
        $scope.showDetail= function (log) {
            $scope.showDetails = true;
            $scope.$broadcast("onShowGalleryViewDetails", log);
        }
        $scope.$on("onHideGalleryViewDetails", function (log) {
            $scope.showDetails = false;
        });
        $scope.pageNumberChanged = function () {
            $scope.gotoPage($scope.currentPageShow);
        };
        $scope.active();
    }]);
})();