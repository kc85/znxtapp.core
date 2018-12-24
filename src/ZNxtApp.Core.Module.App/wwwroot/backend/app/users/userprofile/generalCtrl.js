(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofile.generalCtrl', ['$scope', 'dataService',
    function ($scope, dataService) {
        $scope.$on("onShowUserProfileItem", function (e, menu, user) {
            if (menu.key == "user_profile_general") {
                $scope.userData = angular.copy(user);
            }
        });
        $scope.save = function () {
            var editProfileUrl = "./api/admin/userinfo/update";
            if ($scope.$parent.isShowMyProfile == true) {
                editProfileUrl = "./api/userinfo/update";
            }
            dataService.post(editProfileUrl, $scope.userData).then(function (response) {
                if (response.data.code == 1) {
                    $scope.$emit("onUserInfoUpdate", $scope.userData);
                }
            });
        }
    }]);
})();