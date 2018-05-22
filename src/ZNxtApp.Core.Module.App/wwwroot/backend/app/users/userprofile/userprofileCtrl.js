(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofileCtrl', ['$scope', '$controller', '$window', '$rootScope', 'dataService', 'userData', 'menus',
    function ($scope, $controller, $window, $rootScope, dataService, userData, menus) {
        $scope.user = {};
       
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
    }]);
})();