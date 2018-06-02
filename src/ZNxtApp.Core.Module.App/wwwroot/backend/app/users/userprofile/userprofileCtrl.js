(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofileCtrl', ['$scope', '$controller', '$window', '$rootScope', 'dataService', 'userData', 'menus',
    function ($scope, $controller, $window, $rootScope, dataService, userData, menus) {
        $scope.user = {};
        $scope.loadingUseData = false;
        $scope.userProfileMenus = [];
        var scrollX = 0;
        var scrollY;
        $scope.closeDetails = function () {
            $scope.$emit("onHideUserDetails", $scope.user);
            $window.scrollTo(scrollX, scrollY);
        }
        $scope.$on("onShowUserDetails", function (e, user) {
            $scope.userProfileMenus = menus.filter(function (d) { return d.display_area  == "user_profile_body"});
            $scope.user = user;           
            $scope.active();
            scrollY = $window.scrollY;
            scrollX = $window.scrollX;
            $scope.clickMenu($scope.userProfileMenus[0]);
            $window.scrollTo(0, 0);
        });
        $scope.clickMenu = function (menu) {
            $scope.userProfileMenus.forEach(function (d) { d.isShow = false; });
            menu.isShow = true;
            $scope.$broadcast("onShowUserProfileItem", menu, $scope.user);
        }
        $scope.$on("onUserInfoUpdate", function () {
            $scope.active();
        });
       
        $scope.active = function () {
            if ($scope.loadingUseData == false) {
                $scope.loadingUseData = true;
                $scope.isError = false;
                dataService.get("./api/admin/users?pagesize=1&currentpage=1&filter={'user_id':'" + $scope.user.user_id + "'}").then(function (response) {
                    if (response.data.code == 1) {
                        $scope.user = response.data.data[0];
                    }
                    else {                        
                        $scope.isError = true;
                        $scope.errorMessage = "Something went wrong in the server";
                    }
                    $scope.loadingUseData = false;
                });
            }
        }
        
    }]);
})();