(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofile.homeWidgetsCtrl', ['$scope', '$rootScope', 'dataService', 'menus',
    function ($scope, $rootScope, dataService, menus) {
        $scope.addressCount = 2;
        $scope.groupCount = 3;
        $scope.lastPasswordReset = Date.toString();

        $scope.$on("onShowUserProfileItem", function (e, menu, user) {
            if (menu.key == "info") {
                $scope.userData = user;
                $scope.userProfileWidget = menus.filter(function (d) { return d.display_area == "user_profile_home_widget" });
            }
        });
        $scope.showAddress = function () {
            $scope.$parent.clickMenu({ key: 'user_profile_address' });
        }
    }]);
})();