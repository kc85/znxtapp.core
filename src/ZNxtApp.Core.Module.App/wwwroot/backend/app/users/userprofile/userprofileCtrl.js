(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofileCtrl', ['$scope', '$controller', '$window', '$rootScope', 'dataService', 'userData', 'menus',
    function ($scope, $controller, $window, $rootScope, dataService, userData, menus) {
        $scope.user = {};
        $scope.loadingUseData = false;
        $scope.userProfileMenus = [];
        $scope.showOtherTabs = false;
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
            $scope.clickMenu();
            $window.scrollTo(0, 0);
        });
        $scope.clickMenu = function (menu) {
            $scope.showOtherTabs = true;
            $scope.userProfileMenus.forEach(function (d) { d.isShow = false; });
            if (menu != undefined) {
                // fetch menu from key in case this call from the child controller, e.g homeCtrl
                menu = $scope.userProfileMenus.filter(function (m) { return m.key == menu.key })[0];
                menu.isShow = true;
                $scope.$broadcast("onShowUserProfileItem", menu, $scope.user);
            }
            else {
                $scope.showOtherTabs = false;
                $scope.$broadcast("onShowUserProfileItem", { key  : 'info'}, $scope.user);
            }
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