(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofile.addressCtrl', ['$scope', 'dataService', 'loggerService',
    function ($scope, dataService, logger) {
        $scope.address = {};
        $scope.addresses = [];
        $scope.addNewState = false;
        $scope.showDeleteComment = false;
        $scope.$on("onShowUserProfileItem", function (e, menu, user) {
            if (menu.key == "user_profile_address") {
                console.log(user);
                $scope.userData = angular.copy(user);
                addUserInfo($scope.userData);
                if ($scope.userData.user_info[0].addresses == undefined) {
                    $scope.userData.user_info[0].addresses = [];
                }
                $scope.addresses = angular.copy($scope.userData.user_info[0].addresses);
                if ($scope.userData.user_info[0].addresses.length == 0) {
                    $scope.addNew();
                }
                else {
                    $scope.address = $scope.userData.user_info[0].addresses.filter(function (d) { return d.is_default != false && d.is_deleted != true })[0];
                }
            }
        });
        $scope.save = function (callback) {
            if ($scope.address.is_default) {
                $scope.userData.user_info[0].addresses.filter(function (f) { return f.id != $scope.address.id }).forEach(function (d) { d.is_default = false; })
            }

            var editProfileUrl = "./api/admin/userinfo/update";
            if ($scope.$parent.isShowMyProfile == true) {
                editProfileUrl = "./api/userinfo/update";
            }
            dataService.post(editProfileUrl, $scope.userData).then(function (response) {
                if (response.data.code == 1) {
                    logger.success("Successfully saved the address");
                    $scope.$emit("onUserInfoUpdate", $scope.userData);
                    if ($scope.addNewState) {
                        $scope.addNewState = false;
                        $scope.address = undefined
                    }
                    $scope.addresses = angular.copy($scope.userData.user_info[0].addresses);
                    if (callback != undefined) {
                        callback();
                    }
                }
            });
        }
        function addUserInfo(user) {
            if (user.user_info == undefined) {
                user.user_info = [];
                user.user_info.push({ user_id: user.user_id });
            }
        }
        $scope.select = function (address) {
            $scope.addNewState = false;
            $scope.address = $scope.userData.user_info[0].addresses.filter(function (d) { return d.id == address.id; })[0];
        }
        $scope.addNew = function () {
            $scope.addNewState = true;
            $scope.address = {};
            $scope.address.id = Date().toString();
            $scope.address.is_deleted = false;
            $scope.address.is_default = false;
            $scope.userData.user_info[0].addresses.push($scope.address);
        };
        $scope.delete = function () {
            $scope.showDeleteComment = true;
        }
        $scope.deleteCancel = function () {
            $scope.showDeleteComment = false;
        }

        $scope.deleteAddress = function () {
            $scope.address.is_deleted = true;
            if ($scope.address.is_default == true) {
                var firstAddress = $scope.address.filter(function (a) { return a.is_default != true })[0];
                if (firstAddress != undefined) {
                    firstAddress.is_default = true;
                }
                $scope.address.is_default = false;
            }

            $scope.save(function () {
                $scope.showDeleteComment = false;
                $scope.address = undefined;
            });
        }

        $scope.getNotDeleted = function (item) {
            return item.is_deleted != true;
        }
        $scope.cancel = function () {
            if ($scope.addNewState) {
                $scope.addNewState = false;
                var index = $scope.userData.user_info[0].addresses.indexOf($scope.address);
                if (index != -1) {
                    $scope.userData.user_info[0].addresses.splice(index, 1);
                }
            }
            $scope.address = undefined;
        }
    }]);
})();