(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofile.homeCtrl', ['$scope', '$rootScope', 'dataService',
    function ($scope,$rootScope, dataService) {
       
        $scope.addressCount = 2;
        $scope.groupCount = 3;
        $scope.lastPasswordReset= Date.toString();

        $scope.$on("onShowUserProfileItem", function (e, menu, user) {
            if(menu.key == "info")
            {
                $scope.userData =  user;
            }
        });
        $scope.showAddress = function () {
            $scope.$parent.clickMenu({ key: 'user_profile_address' });
        }
    }]);
})();