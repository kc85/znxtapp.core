(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.userprofileCtrl', ['$scope', '$controller', '$window','$location', '$rootScope', 'dataService', 'userData', 'menus', 'fileUploadService', 'loggerService',
    function ($scope, $controller, $window,$location, $rootScope, dataService, userData, menus, fileUploadService, logger) {
        $scope.user = {};
        $scope.loadingUseData = false;
        $scope.userProfileMenus = [];
        $scope.showOtherTabs = false;
        $scope.user_profile_image = "";
        $scope.isShowMyProfile = false;
        $scope.defaultAddress = {}
        var scrollX = 0;
        var scrollY;

        function onMyProfileLoad() {
            var path = $location.path();
            console.log(path);
            if (path == "/my_profile") {
                $scope.isShowMyProfile = true;
                getUserProfileData();
            }
        }

        onMyProfileLoad();

        $scope.active = function () {
            getUserProfileData();
        }

        function getUserProfileData(callback) {
            if ($scope.loadingUseData == false) {
                $scope.loadingUseData = true;
                $scope.isError = false;
                var getUserInfoUrl = "./api/admin/users?pagesize=1&currentpage=1&filter={'user_id':'" + $scope.user.user_id + "'}";

                if ($scope.isShowMyProfile == true) {
                    getUserInfoUrl = "./api/user/userinfo?user_id=" + userData.user_id + "";
                }
                dataService.get(getUserInfoUrl).then(function (response) {
                    $scope.loadingUseData = false;
                    if (response.data.code == 1) {
                        if ($scope.isShowMyProfile == true) {
                            $scope.user = response.data.data;
                        }
                        else {
                            $scope.user = response.data.data[0];
                        }
                        setUserInfoData($scope.user);
                        if (callback != undefined) {
                            callback();
                        }
                    }
                    else {
                        $scope.isError = true;
                        $scope.errorMessage = "Something went wrong in the server";
                    }
                    
                }, function () {
                    $scope.loadingUseData = false;
                });
            }
        }

        function setUserInfoData(user) {
            $scope.user = user;
            $scope.user_profile_image = "";
            if (user.user_info[0] != undefined) {

                if (user.user_info[0].user_pic_l != undefined) {
                    $scope.user_profile_image = user.user_info[0].user_pic_l.replace("/frontend/", "../");
                }
                else {
                    $scope.user_profile_image = user.user_info[0].user_pic;
                }
                if (user.user_info[0].addresses != undefined) {
                    $scope.defaultAddress = user.user_info[0].addresses.filter(function (d) { return d.is_default == true && d.is_deleted != true })[0];
                }
            }
        }

        function uploadUserImage() {

            if ($scope.userProfileImage != undefined) {
                console.log($scope.userProfileImage);

                var uploadUserImageUrl = "./api/admin/userpic/update?user_id=" + $scope.user.user_id;

                if ($scope.isShowMyProfile) {
                    uploadUserImageUrl = "./api/userpic/update?user_id=" + $scope.user.user_id;
                }

                if ($scope.userProfileImage.type.indexOf("image") != -1) {
                    fileUploadService.uploadFileToUrl($scope.userProfileImage, uploadUserImageUrl, undefined, function (response) {
                        if (response.data.code == 1) {
                            if (response.data.data.user_pic_m != undefined) {
                                $scope.user_profile_image = response.data.data.user_pic_m.replace("/frontend/", "../");
                            }
                            else {
                                $scope.user_profile_image = response.data.data.user_pic;
                            }
                            console.log($scope.user_profile_image);
                            $scope.$emit("onUserInfoUpdate", $scope.user);
                            logger.success("upload image succefully");
                        }
                    }, function () {
                        logger.error("Something worng in server");
                    });
                }
                else {
                    logger.error("Invalid file selection. Please select image file");
                }
            }
        }

        $scope.$on("onShowUserDetails", function (e, user) {
            if ($scope.isShowMyProfile != true) {
                $scope.user = user;
                $scope.userProfileMenus = menus.filter(function (d) { return d.display_area == "user_profile_body" });
                $scope.active();
                scrollY = $window.scrollY;
                scrollX = $window.scrollX;
                $scope.clickMenu();
                $window.scrollTo(0, 0);
            }
        });

        $scope.$on("onUserInfoUpdate", function () {
            getUserProfileData();
        });

        $scope.$watch('userProfileImage', function () {
            uploadUserImage();
        });

        $scope.$on('$locationChangeSuccess', function (event, newUrl, oldUrl) {
            onMyProfileLoad();
        });

        $scope.closeDetails = function () {
            $scope.$emit("onHideUserDetails", $scope.user);
            $window.scrollTo(scrollX, scrollY);
        }

        $scope.clickMenu = function (menu) {
            $scope.showOtherTabs = true;
            $scope.userProfileMenus.forEach(function (d) { d.isShow = false; });
            if (menu != undefined) {
                menu = $scope.userProfileMenus.filter(function (m) { return m.key == menu.key })[0];
                menu.isShow = true;
                $scope.$broadcast("onShowUserProfileItem", menu, $scope.user);
            }
            else {
                $scope.showOtherTabs = false;
                $scope.$broadcast("onShowUserProfileItem", { key: 'info' }, $scope.user);
            }
        }
        
    }]);
})();