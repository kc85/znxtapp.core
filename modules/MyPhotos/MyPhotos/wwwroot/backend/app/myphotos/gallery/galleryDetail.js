(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.myphotos.gallerydetail', ['$scope', '$controller', '$location', '$rootScope', '$window', 'dataService', 'userData', 'loggerService',
    function ($scope, $controller, $location, $rootScope, $window, dataService, userData, logger) {
        $scope.gallery = {};
        $scope.isShow = false;
        var scrollX = 0;
        var scrollY;

        $scope.closeDetails = function () {
            $scope.$emit("onHideGalleryViewDetails", $scope.gallery);
            $window.scrollTo(scrollX, scrollY);
            $scope.isShow = false;
        }
        $scope.$on("onShowGalleryViewDetails", function (e, gallery) {
            $scope.gallery = gallery;
            scrollY = $window.scrollY;
            scrollX = $window.scrollX;
            $window.scrollTo(0, 0);
            $scope.isShow = true;
            active();
        });
        function active() {
            getGallery($scope.gallery.id);
            getAllUsers();
        }
        function getGallery(id) {
            var url = "../api/myphotos/gallery?id=" + id + "&currentpage=0&pagesize=1000";
            dataService.get(url).then(function (response) {
                console.log(response);
                $scope.gallery = response.data.data;
            });
        };
        function getAllUsers() {
            var url = "../api/myphotos/users";
            dataService.get(url).then(function (response) {
                if (response.data.code == 1) {
                    $scope.users = response.data.data;
                    updateAddedUsers();
                }
            });
        }
        function updateAddedUsers() {
            $scope.users.push({ "user_id": "sys_admin", "name": "System Admin", "user_type": "System Admin" });
            $scope.users.push({ "user_id": "*", "name": "Public Access", "user_type": "Public" });
            $scope.users.push({ "user_id": "user", "name": "User Group", "user_type": "Group" });
            $scope.users.forEach(function (d) {
                d.added = false;
                if ($scope.gallery.auth_users.indexOf(d.user_id) != -1) {
                    d.added = true;
                }
            });
        }
        $scope.deleteUser = function (userid) {
            var user = $scope.getUserDetails(userid);
            if (confirm("Are you sure to remove access from user : " + user.name + "?")) {
                user.added = false;
                var index = $scope.gallery.auth_users.indexOf(user.user_id);
                if (index > -1) {
                    $scope.gallery.auth_users.splice(index, 1);
                    $scope.save();
                }
            }
        };
        $scope.addUser = function (userid) {
            var user = $scope.getUserDetails(userid);
            if (confirm("Are you sure to Add access from user : " + user.name + "?")) {
                $scope.gallery.auth_users.push(user.user_id);
                $scope.save();
            }
        };
        $scope.getUserDetails = function (userId) {
            if ($scope.users!=undefined)
            return $scope.users.filter(function (d) { return d.user_id == userId;})[0]
        };

        $scope.save = function () {

            var url = "../api/myphotos/gallery/update?galleryid=" + $scope.gallery.id;
            var data = {};
            data.description = $scope.gallery.description;
            data.display_name = $scope.gallery.display_name;

            // remove duplicate entries;
            let x = (names) => $scope.gallery.auth_users.filter((v, i) => names.indexOf(v) === i)
            data.auth_users = x($scope.gallery.auth_users);

            dataService.post(url, data).then(function (response) {
                if (response.data.code == 1) {
                    alert("Saved Data");
                    active();
                }
            });
        }

    }]);
})();