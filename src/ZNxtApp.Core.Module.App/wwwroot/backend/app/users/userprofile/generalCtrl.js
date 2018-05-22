(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofile.generalCtrl', ['$scope',
    function ($scope) {
       
        $scope.$on("onShowUserProfileItem", function (e, menu, user) {
            if(menu.key == "user_profile_general")
            {
                $scope.userData =  angular.copy(user);
            }
        });
    }]);
})();