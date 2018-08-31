var __myPhotoAppName = 'MyPhotoGalleryApp';

(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, ['infinite-scroll']);

    MyPhotoApp.controller(__myPhotoAppName + '.Main', ['$scope', '$location', '$rootScope', '$http', '$timeout','$window',
       function ($scope, $location, $rootScope, $http, $timeout, $window) {

           $scope.user = undefined;
           $scope.isLoginRequired = false;
           $scope.isShowBookmark = undefined;
           function active() {
               $scope.screenHeight = $window.innerHeight + 30;
               getUserInfo();
               getGallery();
           }
           function getUserInfo() {
               var url = "../api/user/me";
               $http.get(url).then(function (response) {
                   console.log(response)
                   if (response.data.code == 1) {
                       $scope.user = response.data.data;
                   }
                   else if (response.data.code == 401) {
                       console.log("User not Login");
                       $scope.user = undefined;
                       removeCacheKey("/api/myphotos/gallery");
                   }
                   else {
                       alert("Server Error");
                   }
               });
           }
           function getGallery() {
               removeCacheKey("/api/myphotos/gallery", function () {

                   var url = "../api/myphotos/gallery";

                   $http.get(url).then(function (response) {

                       if (response.data.code == 401) {
                           $scope.user = undefined;
                           $scope.gallery = undefined;
                       }
                       else {
                           response.data.data.forEach(function (d) {
                               d.elementfound = false;
                               if (d.thumbnail_image.changeset_no == undefined) {
                                   d.thumbnail_image.changeset_no = 0;
                               }
                               if (d.display_name == undefined) {
                                   d.display_name = $scope.gallery.name;
                               }
                               else if (d.display_name.length == 0) {
                                   d.display_name = $scope.gallery.name;
                               }

                           });
                           $scope.gallery = response.data.data;

                           $(".imagealbum").each(function () {
                               var eleId = $(this).attr("id");
                               var ele = $scope.gallery.filter(function (d) { return ("G_"  + d.id) == eleId })[0];
                               if (ele == undefined) {
                                   $(this).fadeOut();
                               }
                               else {
                                   ele.elementfound = true;
                               }
                           });
                           $http.get("./indexnew.z");
                           blockImageDownload();
                       }
                   });
               });
           }
           active();

       }]);

})();