var __myPhotoAppName = 'MyPhotoGallerySettingsApp';

var __userData = {};
(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, ['infinite-scroll']);

    MyPhotoApp.controller(__myPhotoAppName + '.Main', ['$scope', '$location', '$rootScope', '$window', '$http', '$timeout',
       function ($scope, $location, $rootScope, $window, $http, $timeout) {

           $scope.busy = false;
           $scope.galleryid = GetParameterValues("galleryid");         
           if ($scope.galleryid == undefined) {
               window.location = "./indexnew.z";
           }
           $scope.gallery = undefined         
           function active() {
               getUserInfo();
               
               $scope.loadImages(function () { getAllUsers(); });
           }
           function getUserInfo() {
               var url = "../api/user/me";
               $http.get(url).then(function (response) {
                   if (response.data.code == 1) {
                       $scope.user = response.data.data;
                       __userData = $scope.user;
                   }
                   else {
                       $scope.user = undefined;
                       __userData = undefined;
                       window.location = "./indexnew.z";

                   }
               });
           }
           function getAllUsers() {
               var url = "../api/myphotos/users";
               $http.get(url).then(function (response) {
                   if (response.data.code == 1) {
                       $scope.users = response.data.data;
                       updateAddedUsers();
                   }
               });
           }
           $scope.save = function() {
               var url = "../api/myphotos/gallery/update?galleryid=" +  $scope.galleryid;
               var data = {};
               data.description = $scope.gallery.description;
               data.display_name = $scope.gallery.display_name;
               data.auth_users = $scope.gallery.auth_users;

               $http.post(url, data).then(function (response) {
                   if (response.data.code == 1) {
                       alert("Saved Data");
                   }
               });
           }
           function updateAddedUsers() {
               $scope.users.push({ "user_id" : "*", "name" : "Public Access", "user_type" : "Public"});
               $scope.users.push({ "user_id" : "user", "name" : "User Group", "user_type" : "Group"});
               $scope.users.forEach(function (d) {
                   d.added = false;
                   if ($scope.gallery.auth_users.indexOf(d.user_id) != -1) {
                       d.added = true;
                   }
               });
           }
           $scope.addUser = function (user) {
               user.added = true;
               $scope.gallery.auth_users.push(user.user_id);
           }
           $scope.removeUser = function (user) {
               user.added = false;
               var index = $scope.gallery.auth_users.indexOf(user.user_id);
               if (index > -1) {
                   $scope.gallery.auth_users.splice(index, 1);
               }
           }
           $scope.loadImages = function (callback) {

               if ($scope.busy == false) {
                   $scope.busy = true;
                   fetchPageImage(function () {
                       $scope.busy = false;
                       if (callback != undefined) {
                           callback();
                       }
                   });
               }
           };
           
           function fetchPageImage(callback) {
               removeCacheKey("/api/myphotos/gallery", function () {
                   var url = "../api/myphotos/gallery?id=" + $scope.galleryid + "&currentpage=" + $scope.currentpage + "&pagesize=" + $scope.pagesize;

                   $http.get(url).then(function (response) {
                       if (response.data.code == 401) {
                           $scope.user = response.data.data;
                           showLogin();
                       }
                       else {
                           $scope.gallery = response.data.data;
                           if ($scope.gallery.thumbnail_image.changeset_no == undefined) {
                               $scope.gallery.thumbnail_image.changeset_no = 0;
                           }
                           if ($scope.gallery.display_name == undefined) {
                               $scope.gallery.display_name = $scope.gallery.name;
                           }
                           else if ($scope.gallery.display_name.length == 0) {
                               $scope.gallery.display_name = $scope.gallery.name;
                           }
                           response.data.data.images.forEach(function (d) {
                               if (d.changeset_no == undefined) {
                                   d.changeset_no = 0;
                               }
                               if (d.galleryid == undefined) {
                                   d.galleryid = $scope.galleryid;
                               }
                               $scope.gallery_files.push(d);
                           });
                           $scope.gallery.images = undefined;
                       }
                       if (callback != undefined) {
                           callback();
                       }
                   });
               });

               
           }
           active();

       }]);

})();


function showAddUserDialog() {

    $("#myphotoadduser").modal("show");
};